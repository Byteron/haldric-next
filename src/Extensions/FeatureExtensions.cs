using Godot;
using RelEcs;

public static class GodotExtensions
{
    public static SceneTree GetTree(this ISystem system)
    {
        return system.GetElement<SceneTree>();
    }
}
