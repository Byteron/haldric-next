using Bitron.Ecs;
using Godot;

public struct SpawnFloatingLabelEvent
{
    public Vector3 Position;
    public string Text;
    public Color Color;

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
        if (!world.HasResource<HUDView>())
        {
            return;
        }

        var hudView = world.GetResource<HUDView>();
        var query = world.Query<SpawnFloatingLabelEvent>().End();

        foreach (var eventEntityId in query)
        {
            var spawnEvent = query.Get<SpawnFloatingLabelEvent>(eventEntityId);
            hudView.SpawnFloatingLabel(spawnEvent.Position, spawnEvent.Text, spawnEvent.Color);
        }
    }
}