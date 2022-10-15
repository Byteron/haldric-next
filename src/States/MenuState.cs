using RelEcs;

public class MenuState : IState
{
    public void Enable(World world)
    {
        world.SpawnMainMenu();
    }

    public void Update(World world)
    {
        world.UpdateDebugInfo();
    }

    public void Disable(World world)
    {
        world.DespawnMainMenu();
    }
    
    
}