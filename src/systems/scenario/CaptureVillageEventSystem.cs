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
            
            if (captureEvent.LocEntity.Has<IsCapturedByTeam>())
            {
                captureEvent.LocEntity.Remove<IsCapturedByTeam>();
            }

            captureEvent.LocEntity.Add(new IsCapturedByTeam(captureEvent.Team));
        }
    }
}