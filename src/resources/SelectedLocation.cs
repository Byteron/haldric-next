using Bitron.Ecs;

public class SelectedLocation
{
    public EcsEntity Entity { get; set; }

    public SelectedLocation(EcsEntity entity)
    {
        Entity = entity;
    }
}