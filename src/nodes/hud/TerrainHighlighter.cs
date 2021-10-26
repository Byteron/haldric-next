using Godot;
using System;

public partial class TerrainHighlighter : Node3D
{
    [Export] public PackedScene TerrainHighlight;

    public void PlaceHighlight(Vector3 position, Color color, float scaleFactor)
    {
        var highlight = TerrainHighlight.Instantiate<TerrainHighlight>();
        highlight.Color = color;
        highlight.ScaleFactor = scaleFactor;
        highlight.Position = position;
        AddChild(highlight);
    }

    public void Clear()
    {
        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }
    }
}
