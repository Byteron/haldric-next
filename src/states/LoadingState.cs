using System;
using System.Collections.Generic;
using Godot;
using RelEcs;

public partial class LoadingState : GameState
{
    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new LoadingStateInitSystem());
    }
}

public partial class LoadingStateInitSystem : Resource, ISystem
{
    struct LoadingData
    {
        public string Info { get; set; }
        public Action<Commands> Action { get; set; }

        public LoadingData(string info, Action<Commands> action)
        {
            Info = info;
            Action = action;
        }
    }

    Queue<LoadingData> _loadingStates = new Queue<LoadingData>();
    LoadingStateView _view = null;

    GameState _state;

    Commands _commands;

    public void Run(Commands commands)
    {
        this._commands = commands;

        _loadingStates.Enqueue(new LoadingData("Units", Data.Instance.LoadUnits));
        _loadingStates.Enqueue(new LoadingData("Schedules", Data.Instance.LoadSchedules));
        _loadingStates.Enqueue(new LoadingData("Factions", Data.Instance.LoadFactions));
        _loadingStates.Enqueue(new LoadingData("Terrain", Data.Instance.LoadTerrain));
        _loadingStates.Enqueue(new LoadingData("Maps", Data.Instance.LoadMaps));

        _view = Scenes.Instantiate<LoadingStateView>();

        commands.GetElement<CurrentGameState>().State.AddChild(_view);

        Loading();
    }

    async void Loading()
    {
        var sceneTree = _commands.GetElement<SceneTree>();
        var gameStates = _commands.GetElement<GameStateController>();

        while (_loadingStates.Count > 0)
        {
            var loadingData = _loadingStates.Dequeue();
            _view.Label.Text = $"Loading {loadingData.Info} ...";
            await ToSignal(sceneTree, "process_frame");
            loadingData.Action(_commands);
        }

        gameStates.ChangeState(new MenuState());
    }

    public void CallAction(Action action)
    {
        action();
    }
}
