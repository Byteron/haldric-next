using Godot;
using System;

public partial class EditorView : CanvasLayer
{
    public int BrushSize { get { return (int) _brushSizeSlider.Value; } }
    public int Elevation { get { return (int) _elevationSlider.Value; } }

    HSlider _brushSizeSlider;
    HSlider _elevationSlider;

    public override void _Ready()
    {
        _elevationSlider = GetNode<HSlider>("PanelContainer/VBoxContainer/Elevation/HSlider");
        _brushSizeSlider = GetNode<HSlider>("PanelContainer/VBoxContainer/BrushSize/HSlider");
    }
}
