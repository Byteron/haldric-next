using Godot;
using RelEcs;

public class EditorEditPlayerSystem : ISystem
{
    Coords _previousCoords;

    public World World { get; set; }
    
    public void Run(World world)
    {
        if (!world.TryGetElement<Map>(out var map)) return;
        if (!world.TryGetElement<SelectedTerrain>(out var selectedTerrain)) return;
        if (!world.TryGetElement<HoveredTile>(out var hoveredTile)) return;

        var view = world.GetElement<EditorView>();
        var scene = world.GetCurrentScene();

        if (view.Mode != EditorMode.Player) return;

        var tileEntity = hoveredTile.Entity;

        if (!world.IsAlive(tileEntity)) return;

        var tiles = world.Query<Coords, Elevation, BaseTerrainSlot>();
        var (coords, elevation, baseTerrainSlot) = tiles.Get(tileEntity);

        if (coords == _previousCoords || !Input.IsActionPressed("editor_select")) return;

        _previousCoords = coords;


        var elevationOffset = world.GetComponent<Elevation>(baseTerrainSlot.Entity);

        if (world.HasComponent<IsStartingPositionOfSide>(tileEntity))
        {
            var flagView = world.GetComponent<FlagView>(tileEntity);

            scene.RemoveChild(flagView);
            flagView.QueueFree();

            world.On(tileEntity).Remove<FlagView>().Remove<IsStartingPositionOfSide>();
            view.RemovePlayer(coords);
        }
        else
        {
            var flagView = Scenes.Instantiate<FlagView>();
            scene.AddChild(flagView);
            var pos = coords.ToWorld();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            world.On(tileEntity)
                .Add(flagView)
                .Add(new IsStartingPositionOfSide { Value = view.Players.Count });

            view.AddPlayer(coords);
        }
    }
}