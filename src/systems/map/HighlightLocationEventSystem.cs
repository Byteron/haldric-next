using Bitron.Ecs;

public struct HighlightLocationEvent
{
    public Coords Coords;
    public int Range;

    public HighlightLocationEvent(Coords coords, int range)
    {
        Coords = coords;
        Range = range;
    }
}

public class HighlightLocationsEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<HighlightLocationEvent>().End();

        foreach (var eventEntityId in query)
        {
            var eventData = world.Entity(eventEntityId).Get<HighlightLocationEvent>();

            var shaderData = world.GetResource<ShaderData>();
            shaderData.ResetVisibility(false);

            var cellsInRange = Hex.GetCellsInRange(eventData.Coords.Cube, eventData.Range);

            foreach(var cCell in cellsInRange)
            {
                var nCoords = Coords.FromCube(cCell);
                shaderData.UpdateVisibility((int)nCoords.Offset.x, (int)nCoords.Offset.z, true);
            }

            shaderData.Apply();
        }
    }
}