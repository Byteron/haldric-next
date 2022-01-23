using Bitron.Ecs;
using Godot;

public struct SpawnFloatingLabelEvent
{
    public Vector3 Position { get; set; }
    public string Text { get; set; }
    public Color Color { get; set; }

    public SpawnFloatingLabelEvent(Vector3 position, string text, Color color)
    {
        Position = position;
        Text = text;
        Color = color;
    }
}

public class SpawnFloatingLabelEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var canvas = world.GetResource<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        world.ForEach((ref SpawnFloatingLabelEvent spawnEvent) => 
        {
            var floatingLabel = FloatingLabel.Instantiate(spawnEvent.Position, spawnEvent.Text, spawnEvent.Color);
            canvasLayer.AddChild(floatingLabel);
        });
    }
}