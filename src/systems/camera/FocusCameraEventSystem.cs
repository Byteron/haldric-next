using Bitron.Ecs;
using Godot;

public struct FocusCameraEvent
{
    public Coords Coords;

    public FocusCameraEvent(Coords coords)
    {
        Coords = coords;
    }
}

public class FocusCameraEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<FocusCameraEvent>().End();

        foreach (var eventId in eventQuery)
        {
            if (!world.TryGetResource<CameraOperator>(out var cameraOperator))
            {
                return;
            }

            var eventEntity = world.Entity(eventId);
            ref var focusEvent = ref eventEntity.Get<FocusCameraEvent>();

            cameraOperator.Focus(focusEvent.Coords.World());
        }
    }
}