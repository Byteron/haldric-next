using RelEcs;

public static class GameStateExtensions
{
    public static void PushState(this World world, GameState state)
    {
        world.GetElement<GameStates>().PushState(state);
    }

    public static void PopState(this World world)
    {
        world.GetElement<GameStates>().PopState();
    }

    public static void ChangeState(this World world, GameState state)
    {
        world.GetElement<GameStates>().ChangeState(state);
    }
}