using RelEcs;
using RelEcs.Godot;
using Godot;

public class SpawnFloatingLabelEvent
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

public class SpawnFloatingLabelEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        var canvas = commands.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        commands.Receive((SpawnFloatingLabelEvent spawnEvent) => 
        {
            var floatingLabel = FloatingLabel.Instantiate(spawnEvent.Position, spawnEvent.Text, spawnEvent.Color);
            canvasLayer.AddChild(floatingLabel);
        });
    }
}