using Godot;
using System;

public partial class TurnPanel : Control
{
    [Signal] public delegate void EndTurnButtonPressedEventHandler();

    public Button EndTurnButton { get; private set; }

    public override void _Ready()
    {
        EndTurnButton = GetNode<Button>("EndTurnButton");
    }

     void OnEndTurnButtonPressed()
    {
        EmitSignal(nameof(EndTurnButtonPressedEventHandler));
    }
}
