using System.Collections.Generic;
using Godot;

public partial class GameStates : Node
{
    readonly Stack<GameState> _stack = new();
    readonly World _world;

    public GameStates(World world)
    {
        _world = world;
    }
    
    public override void _Ready()
    {
        Name = "GameStates";
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (_stack.Count == 0) return;

        _world.Send(e);
    }

    public override void _Process(double delta)
    {
        if (_stack.Count == 0) return;

        var currentState = _stack.Peek();
        _world.GetElement<DeltaTime>().Value = delta;
        currentState.Update.Run(_world);

        _world.Tick();
    }

    public override void _ExitTree()
    {
        foreach (var state in _stack)
        {
            state.Exit.Run(_world);
        }
    }

    public void PushState(GameState newState)
    {
        CallDeferred(nameof(PushStateDeferred), newState);
    }

    public void PopState()
    {
        CallDeferred(nameof(PopStateDeferred));
    }

    public void ChangeState(GameState newState)
    {
        CallDeferred(nameof(ChangeStateDeferred), newState);
    }

    void PopStateDeferred()
    {
        if (_stack.Count == 0) return;

        var currentState = _stack.Pop();
        currentState.Exit.Run(_world);
        RemoveChild(currentState);
        currentState.QueueFree();

        if (_stack.Count <= 0) return;

        currentState = _stack.Peek();
        _world.ReplaceElement(currentState);
        currentState.Continue.Run(_world);
    }

    void PushStateDeferred(GameState newState)
    {
        if (_stack.Count > 0)
        {
            var currentState = _stack.Peek();

            if (currentState.GetType() == newState.GetType())
            {
                GD.PrintErr($"{currentState.GetType().ToString()} already at the top of the stack!");
                return;
            }

            currentState.Pause.Run(_world);
        }

        newState.Name = newState.GetType().Name;
        _stack.Push(newState);
        AddChild(newState);

        if (_world.HasElement<GameState>()) _world.ReplaceElement(newState);
        else _world.AddElement(newState);

        newState.Init();
        newState.Enter.Run(_world);
    }

    void ChangeStateDeferred(GameState newState)
    {
        if (_stack.Count > 0)
        {
            var currentState = _stack.Pop();
            currentState.Exit.Run(_world);
            RemoveChild(currentState);
            currentState.QueueFree();
        }

        newState.Name = newState.GetType().Name;
        _stack.Push(newState);
        AddChild(newState);
        _world.ReplaceElement(newState);
        newState.Init();
        newState.Enter.Run(_world);
    }
}