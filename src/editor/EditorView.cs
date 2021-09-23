using Godot;
using System;
using Leopotam.Ecs;

public partial class EditorView : CanvasLayer
{
    public int BrushSize { get { return (int)_brushSizeSlider.Value; } }
    public int Elevation { get { return (int)_elevationSlider.Value; } }

    public bool UseElevation { get { return _elevationCheckBox.Pressed; } }
    public bool UseTerrain { get { return _terrainCheckBox.Pressed; } }

    public EcsEntity TerrainEntity { get { return _selectedTerrain; } }

    EcsEntity _selectedTerrain;

    HSlider _brushSizeSlider;
    HSlider _elevationSlider;

    CheckBox _elevationCheckBox;
    CheckBox _terrainCheckBox;

    Control _terrains;

    TextEdit _widthTextEdit;
    TextEdit _heightTextEdit;


    public override void _Ready()
    {
        _terrains = GetNode<Control>("Tools/VBoxContainer/Terrains/GridContainer");

        _elevationSlider = GetNode<HSlider>("Tools/VBoxContainer/Elevation/HSlider");
        _brushSizeSlider = GetNode<HSlider>("Tools/VBoxContainer/BrushSize/HSlider");

        _elevationCheckBox = GetNode<CheckBox>("Tools/VBoxContainer/Elevation/HBoxContainer/CheckBox");
        _terrainCheckBox = GetNode<CheckBox>("Tools/VBoxContainer/Terrains/HBoxContainer/CheckBox");

        _widthTextEdit = GetNode<TextEdit>("Create/VBoxContainer/HBoxContainer/Width/TextEdit");
        _heightTextEdit = GetNode<TextEdit>("Create/VBoxContainer/HBoxContainer/Height/TextEdit");

        InitializeTerrains();
    }

    public void InitializeTerrains()
    {
        _selectedTerrain = Data.Instance.Terrains["Gg"];

        foreach (var item in Data.Instance.Terrains)
        {
            var code = item.Key;

            var button = new Button();
            button.Text = code;
            button.Connect("pressed", new Callable(this, "OnTerrainSelected"), new Godot.Collections.Array() { code });
            _terrains.AddChild(button);
        }
    }

    public void OnCreateButtonPressed()
    {
        int width = int.Parse(_widthTextEdit.Text);
        int height = int.Parse(_heightTextEdit.Text);

        Main.Instance.World.NewEntity().Replace(new DestroyMapEvent());
        Main.Instance.World.NewEntity().Replace(new CreateMapEvent(width, height));
    }

    public void OnTerrainSelected(string code)
    {
        _selectedTerrain = Data.Instance.Terrains[code];
    }

    public void OnSaveButtonPressed()
    {
        var entity = Main.Instance.World.NewEntity();
        entity.Replace(new SaveMapEvent("map"));
    }

    public void OnLoadButtonPressed()
    {
        var entity = Main.Instance.World.NewEntity();
        entity.Replace(new LoadMapEvent("map"));
    }
}
