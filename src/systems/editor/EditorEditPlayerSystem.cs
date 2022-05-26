using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class EditorEditPlayerSystem : ISystem
{
    Node3D _parent;

    Vector3 _previousCoords;

    public EditorEditPlayerSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<Map>(out var map)) return;
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation)) return;

        var editorView = commands.GetElement<EditorView>();

        if (editorView.Mode != EditorView.EditorMode.Player) return;

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive) return;

        var hoveredCoords = locEntity.Get<Coords>();

        if (hoveredCoords.Cube() == _previousCoords || !Input.IsActionPressed("editor_select")) return;
        
        _previousCoords = hoveredCoords.Cube();

        var coords = locEntity.Get<Coords>();
        var elevation = locEntity.Get<Elevation>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        if (locEntity.Has<IsStartingPositionOfSide>())
        {
            var flagView = locEntity.Get<FlagView>();

            _parent.RemoveChild(flagView);
            flagView.QueueFree();

            locEntity.Remove<FlagView>();
            locEntity.Remove<IsStartingPositionOfSide>();
            editorView.RemovePlayer(coords);
        }
        else
        {
            var flagView = Scenes.Instantiate<FlagView>();
            _parent.AddChild(flagView);
            var pos = coords.World();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            locEntity.Add(flagView);
            locEntity.Add(new IsStartingPositionOfSide { Value = editorView.Players.Count });
            editorView.AddPlayer(coords);
        }
    }
}