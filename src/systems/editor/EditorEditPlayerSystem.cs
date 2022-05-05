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
        if (!commands.TryGetElement<Map>(out var map))
        {
            return;
        }

        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var editorView = commands.GetElement<EditorView>();

        if (editorView.Mode != EditorView.EditorMode.Player)
        {
            return;
        }

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive)
        {
            return;
        }

        var locations = map.Locations;

        ref var hoveredCoords = ref locEntity.Get<Coords>();
        if (hoveredCoords.Cube() != _previousCoords && Input.IsActionPressed("editor_select"))
        {
            _previousCoords = hoveredCoords.Cube();

            ref var coords = ref locEntity.Get<Coords>();
            ref var elevation = ref locEntity.Get<Elevation>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

            if (locEntity.Has<IsStartingPositionOfSide>())
            {
                var handle = locEntity.Get<Node<FlagView>>();

                _parent.RemoveChild(handle.Value);
                handle.Value.QueueFree();
                handle.Value = null;

                locEntity.Remove<Node<FlagView>>();
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

                locEntity.Add(new Node<FlagView>(flagView));
                locEntity.Add(new IsStartingPositionOfSide(editorView.Players.Count));
                editorView.AddPlayer(coords);
            }
        }
    }
}