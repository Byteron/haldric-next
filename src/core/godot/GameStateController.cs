using System.Collections.Generic;
using Godot;

public partial class GameStateController : Node3D
{
    [Signal] public delegate void PostProcessFrame();
    [Signal] public delegate void PreProcessFrame();

    Stack<GameState> _states = new Stack<GameState>();

    public override void _UnhandledInput(InputEvent e)
    {
        if (_states.Count == 0)
        {
            return;
        }

        var currentState = _states.Peek();
        currentState.RunInputSystems();
        currentState.Input(this, e);
    }

    public override void _Process(float delta)
    {
        EmitSignal(nameof(PreProcessFrame));

        if (_states.Count == 0)
        {
            return;
        }

        var currentState = _states.Peek();
        currentState.RunUpdateSystems();
        currentState.RunEventSystems();
        currentState.Update(this, delta);

        EmitSignal(nameof(PostProcessFrame));
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
        PopState();
        PushState(newState);
    }

    private void PopStateDeferred()
    {
        if (_states.Count == 0)
        {
            return;
        }

        var currentState = _states.Pop();
        currentState.RunEventSystems();
        currentState.Exit(this);
        RemoveChild(currentState);
        currentState.QueueFree();

        if (_states.Count > 0)
        {
            currentState = _states.Peek();
            currentState.Continue(this);
        }
    }

    private void PushStateDeferred(GameState newState)
    {
        if (_states.Count > 0)
        {
            var currentState = _states.Peek();

            if (currentState.GetType() == newState.GetType())
            {
                GD.PrintErr($"{currentState.GetType().ToString()} already at the top of the stack!");
                return;
            }
            
            currentState.Pause(this);
        }

        newState.Name = newState.GetType().ToString();
        _states.Push(newState);
        AddChild(newState);
        newState.Enter(this);
    }
}