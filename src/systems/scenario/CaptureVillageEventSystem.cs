using Bitron.Ecs;
using Godot;

public struct CaptureVillageEvent
{
    public EcsEntity LocEntity;
    public int Team;

    public CaptureVillageEvent(EcsEntity locEntity, int team)
    {
        LocEntity = locEntity;
        Team = team;
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

        foreach(var id in query)
        {
            ref var captureEvent = ref query.Get<CaptureVillageEvent>(id);
            
            var locEntity = captureEvent.LocEntity;

            ref var coords = ref locEntity.Get<Coords>();
            ref var elevation = ref locEntity.Get<Elevation>();

            if (locEntity.Has<IsCapturedByTeam>())
            {
                var handle = locEntity.Get<NodeHandle<FlagView>>();
                
                _parent.RemoveChild(handle.Node);
                handle.Node.QueueFree();
                handle.Node = null;

                locEntity.Remove<NodeHandle<FlagView>>();
                locEntity.Remove<IsCapturedByTeam>();
            }

            var flagView = Scenes.Instance.FlagView.Instantiate<FlagView>();
            _parent.AddChild(flagView);
            var pos = coords.World;
            pos.y = elevation.HeightWithOffset;
            flagView.Position = pos;

            locEntity.Add(new NodeHandle<FlagView>(flagView));
            locEntity.Add(new IsCapturedByTeam(captureEvent.Team));
        }
    }
}