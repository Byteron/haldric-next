using Godot;
using RelEcs;

public static class GodotExtensions
{
    public static SceneTree GetTree(this World world)
    {
        return world.GetElement<SceneTree>();
    }

    public static Node3D GetCurrentScene(this World world)
    {
        return (Node3D)world.GetElement<SceneTree>().CurrentScene;
    }
}
