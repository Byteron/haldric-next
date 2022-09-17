using RelEcs;

public class SelectedTile
{
    public Entity Entity { get; set; }

    public SelectedTile(Entity entity)
    {
        Entity = entity;
    }
}