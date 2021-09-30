using Godot;
using Bitron.Ecs;

public class DespawnCameraOperatorSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<NodeHandle<CameraOperator>>().End();

        foreach (var entityId in query)
        {
            world.DespawnEntity(entityId);
        }
    }
}