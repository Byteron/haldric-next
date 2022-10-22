using RelEcs;

public class MenuState : IState
{
    public void Enable(World world)
    {
        SpawnMainMenu(world);
    }

    public void Update(World world)
    {
        UpdateDebugInfo(world);
    }

    public void Disable(World world)
    {
        DespawnMainMenu(world);
    }
}