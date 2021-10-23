using Bitron.Ecs;

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
    public void Run(EcsWorld world)
    {
        var query = world.Query<CaptureVillageEvent>().End();

        foreach(var id in query)
        {
            ref var captureEvent = ref query.Get<CaptureVillageEvent>(id);
            
            if (captureEvent.LocEntity.Has<IsCaptured>())
            {
                captureEvent.LocEntity.Remove<IsCaptured>();
            }

            captureEvent.LocEntity.Add(new IsCaptured(captureEvent.Team));
        }
    }
}