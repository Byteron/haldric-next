using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public class EditorEditPlayerSystem : IEcsSystem
{
    Node3D _parent;

    Vector3 _previousCoords;

    public EditorEditPlayerSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<Map>(out var map))
        {
            return;
        }

        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var editorView = world.GetResource<EditorView>();

        if (editorView.Mode != EditorView.EditorMode.Player)
        {
            return;
        }

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive())
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
                var handle = locEntity.Get<NodeHandle<FlagView>>();

                _parent.RemoveChild(handle.Node);
                handle.Node.QueueFree();
                handle.Node = null;

                locEntity.Remove<NodeHandle<FlagView>>();
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

                locEntity.Add(new NodeHandle<FlagView>(flagView));
                locEntity.Add(new IsStartingPositionOfSide(editorView.Players.Count));
                editorView.AddPlayer(coords);
            }
        }
    }
}