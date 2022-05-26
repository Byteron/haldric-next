using RelEcs;
using RelEcs.Godot;
using Godot;

public class CaptureVillageTrigger
{
    public Entity LocEntity { get; set; }
    public int Side { get; set; }

    public CaptureVillageTrigger(Entity locEntity, int side)
    {
        LocEntity = locEntity;
        Side = side;
    }
}

public class CaptureVillageTriggerSystem : ISystem
{
    Node3D _parent;

    public CaptureVillageTriggerSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        commands.Receive((CaptureVillageTrigger captureEvent) =>
        {
            var locEntity = captureEvent.LocEntity;

            var coords = locEntity.Get<Coords>();
            var elevation = locEntity.Get<Elevation>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            var elevationOffset = terrainEntity.Get<ElevationOffset>();

            if (locEntity.Has<IsCapturedBySide>())
            {
                locEntity.Get<FlagView>().QueueFree();
                locEntity.Remove<FlagView>();
                locEntity.Remove<IsCapturedBySide>();
            }

            var flagView = Scenes.Instantiate<FlagView>();
            flagView.Color = Data.Instance.SideColors[captureEvent.Side];
            _parent.AddChild(flagView);

            var pos = coords.World();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            locEntity.Add(flagView);
            locEntity.Add(new IsCapturedBySide { Value = captureEvent.Side });
        });
    }
}