using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

public partial class GameStateController : Node3D
{
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
        if (_states.Count == 0)
        {
            return;
        }

        var currentState = _states.Peek();
        currentState.RunUpdateSystems();
        currentState.RunEventSystems();
        currentState.Update(this, delta);
    }

    public void PushState(GameState newState)
    {
        newState.Name = newState.GetType().ToString();
        _states.Push(newState);
        AddChild(newState);
        newState.Enter(this);
    }

    public void PopState()
    {
        if (_states.Count == 0)
        {
            return;
        }

        var currentState = _states.Pop();
        RemoveChild(currentState);
        currentState.Exit(this);
        currentState.RunEventSystems();
        currentState.QueueFree();

    }

    public void ChangeState(GameState newState)
    {
        PopState();
        PushState(newState);
    }
}