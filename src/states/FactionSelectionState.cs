using Godot;
using Godot.Collections;
using Bitron.Ecs;

public partial class FactionSelectionState : GameState
{
    private FactionSelectionView _view;

    private string _mapName;
    private Dictionary _mapDict;

    public FactionSelectionState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;
        _mapDict = Data.Instance.Maps[mapName];
    }

    public override void Enter(GameStateController gameStates)
    {   
        _view = Scenes.Instance.FactionSelectionView.Instantiate<FactionSelectionView>();

        var playerDict = (Dictionary)_mapDict["Players"];
        _view.MapName = _mapName;
        _view.PlayerCount = playerDict.Count;
        
        AddChild(_view);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }
}