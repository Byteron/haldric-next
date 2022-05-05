using Godot;
using RelEcs;
using RelEcs.Godot;

public class DespawnCameraOperatorSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.RemoveElement<CameraOperator>();
    }
}