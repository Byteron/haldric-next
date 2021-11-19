using Godot;
using Bitron.Ecs;

public partial class HudView : CanvasLayer
{
    [Signal] public delegate void TurnEndButtonPressed();

    public Label TerrainLabel { get; set; }
    public Label UnitLabel { get; set; }
    public Label PlayerLabel { get; set; }

    public Button TurnEndButton { get; private set; }

    public override void _Ready()
    {
        TerrainLabel = GetNode<Label>("VBoxContainer/TerrainLabel");
        UnitLabel = GetNode<Label>("VBoxContainer2/UnitLabel");
        PlayerLabel = GetNode<Label>("PlayerLabel");
        TurnEndButton = GetNode<Button>("EndTurnButton");
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
        EmitSignal(nameof(TurnEndButtonPressed));
    }
}
