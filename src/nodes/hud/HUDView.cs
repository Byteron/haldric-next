using Godot;
using Bitron.Ecs;

public partial class HudView : CanvasLayer
{
    public Label TerrainLabel;
    public Label UnitLabel;

    public override void _Ready()
    {
        TerrainLabel = GetNode<Label>("VBoxContainer/TerrainLabel");
        UnitLabel = GetNode<Label>("VBoxContainer/UnitLabel");
    }

    public void OnEndTurnButtonPressed()
    {
        Main.Instance.World.Spawn().Add(new TurnEndEvent());
        GD.Print("End Turn");
    }
}