using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Bitron.Ecs;
using System.Threading.Tasks;

public partial class LoadingState : GameState
{
    private struct LoadingData
    {
        public string Info { get; set; }
        public Action Action { get; set; }

        public LoadingData(string info, Action action)
        {
            Info = info;
            Action = action;
        }
    }

    private Queue<LoadingData> _loadingStates = new Queue<LoadingData>();
    private LoadingStateView _view = null;

    public LoadingState(EcsWorld world) : base(world) { }
    
    public override void Enter(GameStateController gameStates)
    {
        _loadingStates.Enqueue(new LoadingData("Units", Data.Instance.LoadUnits));
        _loadingStates.Enqueue(new LoadingData("Terrain", Data.Instance.LoadTerrain));

        _view = Scenes.Instance.LoadingStateView.Instantiate<LoadingStateView>();
        AddChild(_view);
        
        Loading(gameStates);
    }

    async void Loading(GameStateController gameStates)
    {
        while (_loadingStates.Count > 0)
        {
            var loadingData = _loadingStates.Dequeue();
            _view.Label.Text = $"Loading {loadingData.Info} ...";
            await ToSignal(GetTree(), "process_frame");
            loadingData.Action();
        }

        gameStates.ChangeState(new MenuState(_world));
    }

    public void CallAction(Action action)
    {
        action();
    }
}