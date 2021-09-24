using Leopotam.Ecs;

public partial class PlayState : GameState
{
    public PlayState(EcsWorld world) : base(world)
    {
        AddInputSystem(new UpdateMapCursorSystem(this));
        AddInputSystem(new SelectLocationSystem(this));
        AddInputSystem(new CommanderUndoSystem());
        AddInputSystem(new UpdateTerrainInfoSystem());
        AddInputSystem(new LocationHighlightSystem());

        AddUpdateSystem(new MoveUnitSystem());
        AddUpdateSystem(new MoveUnitCommandSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem(this));

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DestroyMapEvent>(new DestroyMapEventSystem());
        AddEventSystem<CreateMapEvent>(new CreateMapEventSystem(this));
        AddEventSystem<CreateUnitEvent>(new CreateUnitEventSystem(this));
    }

    public override void Enter(GameStateController gameStates)
    {
        _world.NewEntity().Replace(new LoadMapEvent("map"));

        _world.NewEntity().Replace(new CreateUnitEvent("Soldier", Coords.FromOffset(5, 5)));
        _world.NewEntity().Replace(new CreateUnitEvent("Soldier", Coords.FromOffset(6, 5)));
        _world.NewEntity().Replace(new CreateUnitEvent("Soldier", Coords.FromOffset(6, 6)));
        _world.NewEntity().Replace(new CreateUnitEvent("Soldier", Coords.FromOffset(6, 7)));
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.NewEntity().Replace(new DestroyMapEvent());
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }
}