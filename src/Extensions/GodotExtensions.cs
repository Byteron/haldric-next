using Godot;
using RelEcs;

public static class GodotExtensions
{
    public static SceneTree GetTree(this ISystem system)
    {
        return system.GetElement<SceneTree>();
    }

    public static Node3D GetCurrentScene(this ISystem system)
    {
        return (Node3D)system.GetElement<SceneTree>().CurrentScene;
    }
}
