public interface IState
{
    void Enable(World world);
    void Update(World world);
    void Disable(World world);
}

public class State<T> where T : IState, new()
{
    static bool enabled;
    static readonly IState Instance = new T();

    public static bool Enable(World world)
    {
        if (enabled) return false;
        enabled = true;
        Instance.Enable(world);
        return true;
    }

    public static bool Disable(World world)
    {
        if (!enabled) return false;
        Instance.Disable(world);
        enabled = false;
        return true;
    }

    public static void Update(World world)
    {
        if (enabled) Instance.Update(world);
    }
}

public static class StateSystems
{
    public static void EnableState<T>(this World world) where T : IState, new()
    {
        State<T>.Enable(world);
    }

    public static void UpdateState<T>(this World world) where T : IState, new()
    {
        State<T>.Update(world);
    }

    public static void DisableState<T>(this World world) where T : IState, new()
    {
        State<T>.Disable(world);
    }
}