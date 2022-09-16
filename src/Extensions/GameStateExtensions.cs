using RelEcs;

public static class GameStateExtensions
{
    public static void PushState(this ISystem system, GameState state)
    {
        system.GetElement<GameStates>().PushState(state);
    }

    public static void PopState(this ISystem system)
    {
        system.GetElement<GameStates>().PopState();
    }

    public static void ChangeState(this ISystem system, GameState state)
    {
        system.GetElement<GameStates>().ChangeState(state);
    }
}