using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public partial class EditorView : CanvasLayer
{
    public enum EditorMode
    {
        Terrain,
        Player,
    }

    public EditorMode Mode { get; set; } = EditorMode.Terrain;

    public int BrushSize => (int)_brushSizeSlider.Value;
    public int Elevation => (int)_elevationSlider.Value;

    public bool UseElevation => _elevationCheckBox.ButtonPressed;
    public bool UseTerrain => _terrainCheckBox.ButtonPressed;

    public Entity TerrainEntity => _selectedTerrain;

    public readonly Dictionary<Coords, int> Players = new();

    public Commands Commands { get; set; }

    Entity _selectedTerrain;

    HSlider _brushSizeSlider;
    HSlider _elevationSlider;

    CheckBox _elevationCheckBox;
    CheckBox _terrainCheckBox;

    Control _terrains;

    TextEdit _widthTextEdit;
    TextEdit _heightTextEdit;

    TextEdit _mapNameTextEdit;

    VBoxContainer _playerContainer;


    public override void _Ready()
    {
        _terrains = GetNode<Control>("Tools/Terrain/Terrains/CenterContainer/GridContainer");

        _elevationSlider = GetNode<HSlider>("Tools/Terrain/Elevation/HSlider");
        _brushSizeSlider = GetNode<HSlider>("Tools/Terrain/BrushSize/HSlider");

        _elevationCheckBox = GetNode<CheckBox>("Tools/Terrain/Elevation/HBoxContainer/CheckBox");
        _terrainCheckBox = GetNode<CheckBox>("Tools/Terrain/Terrains/HBoxContainer/CheckBox");

        _widthTextEdit = GetNode<TextEdit>("Create/VBoxContainer/HBoxContainer/Width/TextEdit");
        _heightTextEdit = GetNode<TextEdit>("Create/VBoxContainer/HBoxContainer/Height/TextEdit");

        _mapNameTextEdit = GetNode<TextEdit>("VBoxContainer/MapTextEdit");

        _playerContainer = GetNode<VBoxContainer>("Tools/Players");

        InitializeTerrains();
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
        foreach (Label child in _playerContainer.GetChildren())
        {
            _playerContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var pair in Players)
        {
            Coords coords = pair.Key;
            int id = pair.Value;

            var label = new Label();
            label.SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill;
            label.MinimumSize = new Vector2(0, 50);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Text = $"Player: {id}, Position: {coords.Offset().x}, {coords.Offset().z}";

            _playerContainer.AddChild(label);
        }
    }

     void InitializeTerrains()
    {
        _selectedTerrain = Data.Instance.Terrains["Gg"];

        foreach (var item in Data.Instance.Terrains)
        {
            var code = item.Key;

            var button = new Button();
            button.MinimumSize = new Vector2(50, 50);
            button.Text = code;
            button.Connect("pressed", new Callable(this, "OnTerrainSelected"), new Godot.Collections.Array() { code });
            _terrains.AddChild(button);
        }
    }

     void OnCreateButtonPressed()
    {
        if (_widthTextEdit.Text.IsValidInteger() && _heightTextEdit.Text.IsValidInteger())
        {
            int width = int.Parse(_widthTextEdit.Text);
            int height = int.Parse(_heightTextEdit.Text);

            Commands.Send(new DespawnMapTrigger());
            Commands.Send(new SpawnMapTrigger(width, height));
        }
        else
        {
            GD.PushWarning("Please specify valid map size!");
            return;
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
        _selectedTerrain = Data.Instance.Terrains[code];
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
            Commands.Send(new SaveMapTrigger(_mapNameTextEdit.Text));
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
            Commands.Send(new DespawnMapTrigger());
            Commands.Send(new LoadMapEvent(_mapNameTextEdit.Text));
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }
}
