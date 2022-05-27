using System;
using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class CurrentGameState
{
    public GameState State;
}

public class GodotInput
{
    public InputEvent Event;
}

public class DeltaTime
{
    public float Value;
}

public partial class GameStateController : Node
{
    Dictionary<Type, GameState> _states = new Dictionary<Type, GameState>();

    Stack<GameState> _stack = new Stack<GameState>();

    RelEcs.World _world = new RelEcs.World();

    Commands _commands;

    public GameStateController()
    {
        _world.AddElement(this);

        _world.AddElement(new CurrentGameState());
        _world.AddElement(new DeltaTime());
    }

    public override void _Ready()
    {
        var tree = GetTree();
        _world.AddElement(tree);

        var m = new Marshallable<Commands>(new Commands(_world, null));
        tree.Root.PropagateCall("_Convert", new Godot.Collections.Array() { m });
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (_stack.Count == 0)
        {
            return;
        }

        var currentState = _stack.Peek();
        // TODO: insert godot event as trigger
        currentState.InputSystems.Run(_world);
    }

    public override void _Process(float delta)
    {
        if (_stack.Count == 0)
        {
            return;
        }

        var currentState = _stack.Peek();
        _world.GetElement<DeltaTime>().Value = delta;
        currentState.UpdateSystems.Run(_world);

        _world.Tick();
    }

    public override void _ExitTree()
    {
        foreach (var state in _stack)
        {
            state.ExitSystems.Run(_world);
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
        if (_stack.Count == 0)
        {
            return;
        }

        var currentState = _stack.Pop();
        currentState.ExitSystems.Run(_world);
        RemoveChild(currentState);
        currentState.QueueFree();

        if (_stack.Count > 0)
        {
            currentState = _stack.Peek();
            _world.GetElement<CurrentGameState>().State = currentState;
            currentState.ContinueSystems.Run(_world);
        }
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

            currentState.PauseSystems.Run(_world);
        }

        newState.Name = newState.GetType().ToString();
        _stack.Push(newState);
        AddChild(newState);
        _world.GetElement<CurrentGameState>().State = newState;
        newState.Init(this);
        newState.InitSystems.Run(_world);
    }

    void ChangeStateDeferred(GameState newState)
    {
        if (_stack.Count > 0)
        {
            var currentState = _stack.Pop();
            currentState.ExitSystems.Run(_world);
            RemoveChild(currentState);
            currentState.QueueFree();
        }

        newState.Name = newState.GetType().ToString();
        _stack.Push(newState);
        AddChild(newState);
        _world.GetElement<CurrentGameState>().State = newState;
        newState.Init(this);
        newState.InitSystems.Run(_world);
    }
}