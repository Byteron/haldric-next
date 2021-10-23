using Godot;
using Bitron.Ecs;

public partial class HUDView : CanvasLayer
{
    public Label TerrainLabel;
    public Label UnitLabel;
    public Label PlayerLabel;

    public override void _Ready()
    {
        TerrainLabel = GetNode<Label>("VBoxContainer/TerrainLabel");
        UnitLabel = GetNode<Label>("VBoxContainer2/UnitLabel");
        PlayerLabel = GetNode<Label>("PlayerLabel");
    }

    public void SpawnFloatingLabel(Vector3 position, string text, Color color)
    {
        var label = Scenes.Instance.FloatingLabel.Instantiate<FloatingLabel>();
        label.Position = position;
        label.Text = text;
        label.Color = color;
        AddChild(label);
    }

    public UnitPlate CreateUnitPlate()
    {
        var plate = Scenes.Instance.UnitPlate.Instantiate<UnitPlate>();
        AddChild(plate);
        return plate;
    }

    private void OnEndTurnButtonPressed()
    {
        Main.Instance.World.Spawn().Add(new TurnEndEvent());
    }
}