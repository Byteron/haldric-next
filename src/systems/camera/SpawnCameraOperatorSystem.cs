using Godot;
using Bitron.Ecs;

public class SpawnCameraOperatorSystem : IEcsSystem
{
    Node3D _parent;

    public SpawnCameraOperatorSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var cameraEntity = world.Spawn();

        var cameraOperator = Scenes.Instance.CameraOperator.Instantiate<CameraOperator>();
        _parent.AddChild(cameraOperator);

        cameraEntity.Add(new NodeHandle<CameraOperator>(cameraOperator));
    }
}