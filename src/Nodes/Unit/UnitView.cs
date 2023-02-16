using Godot;
using System;

public partial class UnitView : Node3D
{
    AnimationTree _animTree;
    AnimationNodeStateMachinePlayback _stateMachine;

    public override void _Ready()
    {
        _animTree = GetNodeOrNull<AnimationTree>("AnimationTree");
        _stateMachine = _animTree.Get("parameters/playback").AsGodotObject() as AnimationNodeStateMachinePlayback;
        Play("Attack");
    }

    public void Play(string animName)
    {
        if (_animTree == null || _animTree.AnimPlayer == null || _stateMachine == null)
        {
            return;
        }

        if (_stateMachine.IsPlaying())
        {
            _stateMachine.Travel(animName);
            GD.Print("Travel to Anim: " + animName);
        }
        else
        {
            _stateMachine.Start(animName);
            GD.Print("Start with Anim: " + animName);
        }
    }
}
