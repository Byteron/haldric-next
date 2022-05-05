using RelEcs;
using RelEcs.Godot;

public struct FocusCameraEvent
{
    public Coords Coords;

    public FocusCameraEvent(Coords coords)
    {
        Coords = coords;
    }
}

public class FocusCameraEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<CameraOperator>(out var cameraOperator))
        {
            return;
        }

        commands.Receive((FocusCameraEvent focusEvent) =>
        {
            cameraOperator.Focus(focusEvent.Coords.World());
        });
    }
}