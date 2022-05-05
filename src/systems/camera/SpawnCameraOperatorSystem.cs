using Godot;
using RelEcs;
using RelEcs.Godot;

public class SpawnCameraOperatorSystem : ISystem
{
    Node3D _parent;

    public SpawnCameraOperatorSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        var cameraOperator = Scenes.Instantiate<CameraOperator>();
        _parent.AddChild(cameraOperator);

        commands.AddElement(cameraOperator);
    }
}