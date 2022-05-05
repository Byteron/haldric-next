using System;
using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class CurrentGameState
{
    public GameState State;
}

public struct GodotInput
{
    public InputEvent Event;
}

public class DeltaTime
{
    public float Value;
}

public partial class GameStateController : Node
{
    Dictionary<Type, GameState> states = new Dictionary<Type, GameState>();

    Stack<GameState> stack = new Stack<GameState>();

    RelEcs.World world = new RelEcs.World();

    Commands commands;

    public GameStateController()
    {
        world.AddElement(this);

        world.AddElement(new CurrentGameState());
        world.AddElement(new DeltaTime());
    }

    public override void _Ready()
    {
        var tree = GetTree();
        world.AddElement(tree);

        var m = new Marshallable<Commands>(new Commands(world, null));
        tree.Root.PropagateCall("_Convert", new Godot.Collections.Array() { m });
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (stack.Count == 0)
        {
            return;
        }

        var currentState = stack.Peek();
        // TODO: insert godot event as trigger
        currentState.InputSystems.Run(world);
    }

    public override void _Process(float delta)
    {
        if (stack.Count == 0)
        {
            return;
        }

        var currentState = stack.Peek();
        world.GetElement<DeltaTime>().Value = delta;
        currentState.UpdateSystems.Run(world);

        world.Tick();
    }

    public override void _ExitTree()
    {
        foreach (var state in stack)
        {
            state.ExitSystems.Run(world);
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
        if (stack.Count == 0)
        {
            return;
        }

        var currentState = stack.Pop();
        currentState.ExitSystems.Run(world);
        RemoveChild(currentState);
        currentState.QueueFree();

        if (stack.Count > 0)
        {
            currentState = stack.Peek();
            world.GetElement<CurrentGameState>().State = currentState;
            currentState.ContinueSystems.Run(world);
        }
    }

    void PushStateDeferred(GameState newState)
    {
        if (stack.Count > 0)
        {
            var currentState = stack.Peek();

            if (currentState.GetType() == newState.GetType())
            {
                GD.PrintErr($"{currentState.GetType().ToString()} already at the top of the stack!");
                return;
            }

            currentState.PauseSystems.Run(world);
        }

        newState.Name = newState.GetType().ToString();
        stack.Push(newState);
        AddChild(newState);
        world.GetElement<CurrentGameState>().State = newState;
        newState.Init(this);
        newState.InitSystems.Run(world);
    }

    void ChangeStateDeferred(GameState newState)
    {
        if (stack.Count > 0)
        {
            var currentState = stack.Pop();
            currentState.ExitSystems.Run(world);
            RemoveChild(currentState);
            currentState.QueueFree();
        }

        newState.Name = newState.GetType().ToString();
        stack.Push(newState);
        AddChild(newState);
        world.GetElement<CurrentGameState>().State = newState;
        newState.Init(this);
        newState.InitSystems.Run(world);
    }
}