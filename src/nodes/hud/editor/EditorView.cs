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

    public int BrushSize { get { return (int)brushSizeSlider.Value; } }
    public int Elevation { get { return (int)elevationSlider.Value; } }

    public bool UseElevation { get { return elevationCheckBox.Pressed; } }
    public bool UseTerrain { get { return terrainCheckBox.Pressed; } }

    public Entity TerrainEntity { get { return selectedTerrain; } }

    public Dictionary<Coords, int> Players = new Dictionary<Coords, int>();

    public Commands Commands { get; set; }

    Entity selectedTerrain;

    HSlider brushSizeSlider;
    HSlider elevationSlider;

    CheckBox elevationCheckBox;
    CheckBox terrainCheckBox;

    Control terrains;

    TextEdit widthTextEdit;
    TextEdit heightTextEdit;

    TextEdit mapNameTextEdit;

    VBoxContainer playerContainer;


    public override void _Ready()
    {
        terrains = GetNode<Control>("Tools/Terrain/Terrains/CenterContainer/GridContainer");

        elevationSlider = GetNode<HSlider>("Tools/Terrain/Elevation/HSlider");
        brushSizeSlider = GetNode<HSlider>("Tools/Terrain/BrushSize/HSlider");

        elevationCheckBox = GetNode<CheckBox>("Tools/Terrain/Elevation/HBoxContainer/CheckBox");
        terrainCheckBox = GetNode<CheckBox>("Tools/Terrain/Terrains/HBoxContainer/CheckBox");

        widthTextEdit = GetNode<TextEdit>("Create/VBoxContainer/HBoxContainer/Width/TextEdit");
        heightTextEdit = GetNode<TextEdit>("Create/VBoxContainer/HBoxContainer/Height/TextEdit");

        mapNameTextEdit = GetNode<TextEdit>("VBoxContainer/MapTextEdit");

        playerContainer = GetNode<VBoxContainer>("Tools/Players");

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
        foreach (Label child in playerContainer.GetChildren())
        {
            playerContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var pair in Players)
        {
            Coords coords = pair.Key;
            int id = pair.Value;

            var label = new Label();
            label.SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill;
            label.RectMinSize = new Vector2(0, 50);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Text = $"Player: {id}, Position: {coords.Offset().x}, {coords.Offset().z}";

            playerContainer.AddChild(label);
        }
    }

     void InitializeTerrains()
    {
        selectedTerrain = Data.Instance.Terrains["Gg"];

        foreach (var item in Data.Instance.Terrains)
        {
            var code = item.Key;

            var button = new Button();
            button.RectMinSize = new Vector2(50, 50);
            button.Text = code;
            button.Connect("pressed", new Callable(this, "OnTerrainSelected"), new Godot.Collections.Array() { code });
            terrains.AddChild(button);
        }
    }

     void OnCreateButtonPressed()
    {
        if (widthTextEdit.Text.IsValidInteger() && heightTextEdit.Text.IsValidInteger())
        {
            int width = int.Parse(widthTextEdit.Text);
            int height = int.Parse(heightTextEdit.Text);

            Commands.Send(new DespawnMapEvent());
            Commands.Send(new SpawnMapEvent(width, height));
        }
        else
        {
            GD.PushWarning("Please specify valid map size!");
            return;
        }
    }

     void OnToolsTabChanged(int index)
    {
        if (index == 0)
        {
            Mode = EditorMode.Terrain;
        }
        else if (index == 1)
        {
            Mode = EditorMode.Player;
        }
    }

     void OnTerrainSelected(string code)
    {
        selectedTerrain = Data.Instance.Terrains[code];
    }

     void OnSaveButtonPressed()
    {
        if (string.IsNullOrEmpty(mapNameTextEdit.Text))
        {
            GD.PushWarning("Invalid Map Name: Please specify a Map Name");
            return;
        }

        if (mapNameTextEdit.Text.IsValidIdentifier())
        {
            Commands.Send(new SaveMapEvent(mapNameTextEdit.Text));
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }

     void OnLoadButtonPressed()
    {
        if (string.IsNullOrEmpty(mapNameTextEdit.Text))
        {
            GD.PushWarning("Invalid Map Name: Please specify a Map Name");
            return;
        }

        if (mapNameTextEdit.Text.IsValidIdentifier())
        {
            Commands.Send(new DespawnMapEvent());
            Commands.Send(new LoadMapEvent(mapNameTextEdit.Text));
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }
}
