using RelEcs;
using Godot;

public static class UISystems
{
    public static void SpawnFloatingLabel(World world, Vector3 position, string text, Color color)
    {
        var canvas = world.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var floatingLabel = FloatingLabel.Instantiate(position, text, color);
        canvasLayer.AddChild(floatingLabel);
    }
}