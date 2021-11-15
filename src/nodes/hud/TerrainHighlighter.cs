using Godot;
using System.Collections.Generic;

public partial class TerrainHighlighter : Node3D
{
    [Export] public PackedScene TerrainHighlight;
    [Export] public PackedScene Border3D;

    public List<Border3D> _borders = new List<Border3D>();
    public List<TerrainHighlight> _path = new List<TerrainHighlight>();

    public void ClearPath()
    {
        foreach (var child in _path)
        {
            RemoveChild(child);
            child.QueueFree();
        }

        _path.Clear();
    }
    public void ShowPath(Path path)
    {
        foreach (var pLocEntity in path.Checkpoints)
        {
            ref var coords = ref pLocEntity.Get<Coords>();
            ref var elevation = ref pLocEntity.Get<Elevation>();

            var pos = coords.World();
            pos.y = elevation.Height;

            PlaceHighlight(pos, new Color("FFFFFF"), 0.7f);
        }
    }

    public void PlaceHighlight(Vector3 position, Color color, float scaleFactor)
    {
        var highlight = TerrainHighlight.Instantiate<TerrainHighlight>();
        highlight.Color = color;
        highlight.ScaleFactor = scaleFactor;
        highlight.Position = position;
        AddChild(highlight);
        _path.Add(highlight);
    }

    public void PlaceBorder(Vector3 position, Color color, float rotate, float scaleFactor)
    {
        Border3D highlight = Border3D.Instantiate<Border3D>();
        highlight.Color = color;
        highlight.ScaleFactor = scaleFactor;
        highlight.Direction = rotate;
        highlight.Position = position;
        AddChild(highlight);
        _borders.Add(highlight);
    }

    public void Clear()
    {
        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }
        _path.Clear();
        _borders.Clear();
    }
}
