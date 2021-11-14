using Godot;
using Bitron.Ecs;

public class DespawnCameraOperatorSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.RemoveResource<CameraOperator>();
    }
}