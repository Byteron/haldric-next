using Godot;
using System;
using Leopotam.Ecs;

public partial class EditorView : CanvasLayer
{
    public int BrushSize { get { return (int)_brushSizeSlider.Value; } }
    public int Elevation { get { return (int)_elevationSlider.Value; } }

    public EcsEntity TerrainEntity { get { return _selectedTerrain; } }

    EcsEntity _selectedTerrain;

    HSlider _brushSizeSlider;
    HSlider _elevationSlider;

    Control _terrains;

    public override void _Ready()
    {
        _terrains = GetNode<Control>("Terrains/VBoxContainer");

        _elevationSlider = GetNode<HSlider>("PanelContainer/VBoxContainer/Elevation/HSlider");
        _brushSizeSlider = GetNode<HSlider>("PanelContainer/VBoxContainer/BrushSize/HSlider");

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

    public void OnTerrainSelected(string code)
    {
        _selectedTerrain = Data.Instance.Terrains[code];
    }
}
