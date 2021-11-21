using Godot;
using System;

public partial class TurnPanel : Control
{
    [Signal] public delegate void EndTurnButtonPressed();

    public Button EndTurnButton { get; private set; }

    public override void _Ready()
    {
        EndTurnButton = GetNode<Button>("EndTurnButton");
    }

    private void OnEndTurnButtonPressed()
    {
        EmitSignal(nameof(EndTurnButtonPressed));
    }
}
