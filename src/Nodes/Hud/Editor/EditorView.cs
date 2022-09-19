using System.Collections.Generic;
using Godot;
using RelEcs;

public enum EditorMode
{
    Terrain,
    Player,
}

public partial class EditorView : CanvasLayer
{
    public EditorMode Mode = EditorMode.Terrain;

    public readonly Dictionary<Coords, int> Players = new();

    [Export] public Button CreateButton;
    [Export] public Button SaveButton;
    [Export] public Button LoadButton;

    [Export] public TabContainer ToolsTab;
    
    [Export] public HSlider BrushSizeSlider;
    [Export] public HSlider ElevationSlider;

    [Export] public CheckBox ElevationCheckBox;
    [Export] public CheckBox TerrainCheckBox;

    [Export] public Control Terrains;

    [Export] public TextEdit WidthTextEdit;
    [Export] public TextEdit HeightTextEdit;

    [Export] public TextEdit MapNameTextEdit;

    [Export] public VBoxContainer PlayerContainer;

    public int BrushSize => (int)BrushSizeSlider.Value;
    public int Elevation => (int)ElevationSlider.Value;

    public bool UseElevation => ElevationCheckBox.ButtonPressed;
    public bool UseTerrain => TerrainCheckBox.ButtonPressed;

    public void AddPlayer(Coords coords)
    {
        Players.Add(coords, Players.Count);
        UpdatePlayers();
    }

    public void RemovePlayer(Coords coords)
    {
        Players.Remove(coords);
        UpdatePlayers();
    }

    void UpdatePlayers()
    {
        foreach (var child in PlayerContainer.GetChildren())
        {
            PlayerContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var (coords, id) in Players)
        {
            var label = new Label();
            label.SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill;
            label.CustomMinimumSize = new Vector2i(0, 50);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Text = $"Player: {id}, Position: {coords.ToOffset().x}, {coords.ToOffset().z}";

            PlayerContainer.AddChild(label);
        }
    }
}