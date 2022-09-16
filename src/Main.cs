global using World = RelEcs.World;
using Godot;

public class DeltaTime
{
    public double Value;
}

public partial class Main : Node3D
{
    readonly World _world = new();
    GameStates _gameStates;

    public override void _Ready()
    {
        _gameStates = new GameStates(_world);
        AddChild(_gameStates);

        _world.AddElement(GetTree());
        _world.AddElement(_gameStates);
        _world.AddElement(new DeltaTime());

        _gameStates.PushState(new AppState());
    }
}