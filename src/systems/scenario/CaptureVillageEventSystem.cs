using RelEcs;
using RelEcs.Godot;
using Godot;

public struct CaptureVillageEvent
{
    public Entity LocEntity { get; set; }
    public int Side { get; set; }

    public CaptureVillageEvent(Entity locEntity, int side)
    {
        LocEntity = locEntity;
        Side = side;
    }
}

public class CaptureVillageEventSystem : ISystem
{
    Node3D _parent;

    public CaptureVillageEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        commands.Receive((CaptureVillageEvent captureEvent) =>
        {
            var locEntity = captureEvent.LocEntity;

            ref var coords = ref locEntity.Get<Coords>();
            ref var elevation = ref locEntity.Get<Elevation>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

            if (locEntity.Has<IsCapturedBySide>())
            {
                var handle = locEntity.Get<Node<FlagView>>();
                locEntity.Remove<Node<FlagView>>();
                locEntity.Remove<IsCapturedBySide>();
            }

            var flagView = Scenes.Instantiate<FlagView>();
            flagView.Color = Data.Instance.SideColors[captureEvent.Side];
            _parent.AddChild(flagView);

            var pos = coords.World();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            locEntity.Add(new Node<FlagView>(flagView));
            locEntity.Add(new IsCapturedBySide(captureEvent.Side));
        });
    }
}