global using World = RelEcs.World;
using Godot;

public class DeltaTime
{
    public double Value;
}

public partial class Main : Node3D
{
    readonly World _world = new();

    public override void _Ready()
    {

        _world.AddElement(GetTree());
        _world.AddElement(new DeltaTime());
        
        _world.EnableState<AppState>();
    }

    public override void _Process(double delta)
    {
        _world.GetElement<DeltaTime>().Value = delta;
        
        _world.UpdateState<AppState>();
        _world.UpdateState<MenuState>();
        _world.UpdateState<TestState>();
        _world.UpdateState<EditorState>();
    }
}