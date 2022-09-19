using Godot;
using RelEcs;

public class EditorEditPlayerSystem : ISystem
{
    Coords _previousCoords;

    public World World { get; set; }
    
    public void Run()
    {
        if (!this.TryGetElement<Map>(out var map)) return;
        if (!this.TryGetElement<SelectedTerrain>(out var selectedTerrain)) return;
        if (!this.TryGetElement<HoveredTile>(out var hoveredTile)) return;

        var view = this.GetElement<EditorView>();
        var scene = this.GetCurrentScene();

        if (view.Mode != EditorMode.Player) return;

        var tileEntity = hoveredTile.Entity;

        if (!this.IsAlive(tileEntity)) return;

        var tiles = this.Query<Coords, Elevation, BaseTerrainSlot>();
        var (coords, elevation, baseTerrainSlot) = tiles.Get(tileEntity);

        if (coords == _previousCoords || !Input.IsActionPressed("editor_select")) return;

        _previousCoords = coords;


        var elevationOffset = this.GetComponent<Elevation>(baseTerrainSlot.Entity);

        if (this.HasComponent<IsStartingPositionOfSide>(tileEntity))
        {
            var flagView = this.GetComponent<FlagView>(tileEntity);

            scene.RemoveChild(flagView);
            flagView.QueueFree();

            this.On(tileEntity).Remove<FlagView>().Remove<IsStartingPositionOfSide>();
            view.RemovePlayer(coords);
        }
        else
        {
            var flagView = Scenes.Instantiate<FlagView>();
            scene.AddChild(flagView);
            var pos = coords.ToWorld();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            this.On(tileEntity)
                .Add(flagView)
                .Add(new IsStartingPositionOfSide { Value = view.Players.Count });

            view.AddPlayer(coords);
        }
    }
}