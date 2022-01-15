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
        var cameraOperator = Scenes.Instantiate<CameraOperator>();
        _parent.AddChild(cameraOperator);

        world.AddResource(cameraOperator);
    }
}