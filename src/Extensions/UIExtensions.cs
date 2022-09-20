using RelEcs;
using Godot;

public static class UIExtensions
{
    public static void SpawnFloatingLabel(this ISystem system, Vector3 position, string text, Color color)
    {
        var canvas = system.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var floatingLabel = FloatingLabel.Instantiate(position, text, color);
        canvasLayer.AddChild(floatingLabel);
    }
}