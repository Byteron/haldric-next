using Bitron.Ecs;
using Godot;

public struct CaptureVillageEvent
{
    public EcsEntity LocEntity { get; set; }
    public int Side { get; set; }

    public CaptureVillageEvent(EcsEntity locEntity, int side)
    {
        LocEntity = locEntity;
        Side = side;
    }
}

public class CaptureVillageEventSystem : IEcsSystem
{
    Node3D _parent;

    public CaptureVillageEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var query = world.Query<CaptureVillageEvent>().End();

        foreach (var id in query)
        {
            ref var captureEvent = ref world.Entity(id).Get<CaptureVillageEvent>();

            var locEntity = captureEvent.LocEntity;

            ref var coords = ref locEntity.Get<Coords>();
            ref var elevation = ref locEntity.Get<Elevation>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

            if (locEntity.Has<IsCapturedBySide>())
            {
                var handle = locEntity.Get<NodeHandle<FlagView>>();

                _parent.RemoveChild(handle.Node);
                handle.Node.QueueFree();
                handle.Node = null;

                locEntity.Remove<NodeHandle<FlagView>>();
                locEntity.Remove<IsCapturedBySide>();
            }

            var flagView = Scenes.Instance.FlagView.Instantiate<FlagView>();
            flagView.Color = Data.Instance.SideColors[captureEvent.Side];
            _parent.AddChild(flagView);

            var pos = coords.World();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            locEntity.Add(new NodeHandle<FlagView>(flagView));
            locEntity.Add(new IsCapturedBySide(captureEvent.Side));
        }
    }
}