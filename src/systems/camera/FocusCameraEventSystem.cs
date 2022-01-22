using Bitron.Ecs;

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
        if (!world.TryGetResource<CameraOperator>(out var cameraOperator))
        {
            return;
        }

        world.ForEach((ref FocusCameraEvent focusEvent) =>
        {
            cameraOperator.Focus(focusEvent.Coords.World());
        });
    }
}