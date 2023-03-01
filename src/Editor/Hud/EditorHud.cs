using System.Collections.Generic;
using Godot;

namespace Haldric;

public partial class EditorHud : CanvasLayer
{
    public enum EditorMode
    {
        Terrain,
        Player,
    }

    [Signal] public delegate void CreateButtonPressedEventHandler(int width, int height);
    [Signal] public delegate void SaveButtonPressedEventHandler(string name);
    [Signal] public delegate void LoadButtonPressedEventHandler(string name);
    
    public EditorMode Mode { get; set; } = EditorMode.Terrain;

    public int BrushSize => (int)_brushSizeSlider.Value;
    public int Elevation => (int)_elevationSlider.Value;

    public bool UseElevation => _elevationCheckBox.ButtonPressed;
    public bool UseTerrain => _terrainCheckBox.ButtonPressed;

    public readonly Dictionary<Coords, int> Players = new();
    
    public Terrain SelectedTerrain = default!;

    [Export] HSlider _brushSizeSlider = default!;
    [Export] HSlider _elevationSlider = default!;
    [Export] CheckBox _elevationCheckBox = default!;
    [Export] CheckBox _terrainCheckBox = default!;
    [Export] Control _terrainContainer = default!;
    [Export] TextEdit _withTextEdit = default!;
    [Export] TextEdit _heightTextEdit = default!;
    [Export] TextEdit _mapNameTextEdit = default!;
    [Export] VBoxContainer _playerContainer = default!;

    public override void _Ready()
    {
        InitializeTerrains();
        OnTerrainSelected("Gg");
    }

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
        foreach (var node in _playerContainer.GetChildren())
        {
            var child = (Label)node;
            _playerContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var (coords, id) in Players)
        {
            var label = new Label();
            label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            label.CustomMinimumSize = new Vector2(0, 50);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Text = $"Player: {id}, Position: {coords.ToOffset().X}, {coords.ToOffset().Z}";

            _playerContainer.AddChild(label);
        }
    }

    void InitializeTerrains()
    {
        SelectedTerrain = Data.Instance.Terrains["Gg"];

        foreach (var item in Data.Instance.Terrains)
        {
            var code = item.Key;

            var button = new Button();
            button.CustomMinimumSize = new Vector2(50, 50);
            button.Text = code;
            button.Pressed += () => { OnTerrainSelected(code); };
            _terrainContainer.AddChild(button);
        }
    }

    void OnCreateButtonPressed()
    {
        if (_withTextEdit.Text.IsValidInt() && _heightTextEdit.Text.IsValidInt())
        {
            var width = int.Parse(_withTextEdit.Text);
            var height = int.Parse(_heightTextEdit.Text);
            EmitSignal(SignalName.CreateButtonPressed, width, height);
        }
        else
        {
            GD.PushWarning("Please specify valid map size!");
        }
    }

    void OnToolsTabChanged(int index)
    {
        Mode = index switch
        {
            0 => EditorMode.Terrain,
            1 => EditorMode.Player,
            _ => Mode
        };
    }

    void OnTerrainSelected(string code)
    {
        SelectedTerrain = Data.Instance.Terrains[code];
    }

    void OnSaveButtonPressed()
    {
        if (string.IsNullOrEmpty(_mapNameTextEdit.Text))
        {
            GD.PushWarning("Invalid Map Name: Please specify a Map Name");
            return;
        }

        if (_mapNameTextEdit.Text.IsValidIdentifier())
        {
            EmitSignal(SignalName.SaveButtonPressed, _mapNameTextEdit.Text);
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }

    void OnLoadButtonPressed()
    {
        if (string.IsNullOrEmpty(_mapNameTextEdit.Text))
        {
            GD.PushWarning("Invalid Map Name: Please specify a Map Name");
            return;
        }

        if (_mapNameTextEdit.Text.IsValidIdentifier())
        {
            EmitSignal(SignalName.LoadButtonPressed, _mapNameTextEdit.Text);
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }
}