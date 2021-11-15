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
        if (!world.HasResource<HudView>())
        {
            return;
        }

        var hudView = world.GetResource<HudView>();
        var query = world.Query<SpawnFloatingLabelEvent>().End();

        foreach (var eventEntityId in query)
        {
            var spawnEvent = world.Entity(eventEntityId).Get<SpawnFloatingLabelEvent>();
            hudView.SpawnFloatingLabel(spawnEvent.Position, spawnEvent.Text, spawnEvent.Color);
        }
    }
}