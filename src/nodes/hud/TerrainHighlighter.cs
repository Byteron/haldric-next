using Godot;
using System.Collections.Generic;

public partial class TerrainHighlighter : Node3D
{
    [Export] public PackedScene TerrainHighlight;
    public List<TerrainHighlight> _path = new List<TerrainHighlight>();

    public void ShowPath(Path path)
    {
        foreach (var child in _path)
        {
            RemoveChild(child);
            child.QueueFree();
        }

        _path.Clear();

        foreach(var pLocEntity in path.Checkpoints)
        {
            ref var coords = ref pLocEntity.Get<Coords>();
            ref var elevation = ref pLocEntity.Get<Elevation>();

            var pos = coords.World;
            pos.y = elevation.Height;

            PlaceHighlight(pos, new Color("FFFFFF"), 0.7f, true);
        }
    }

    public void PlaceHighlight(Vector3 position, Color color, float scaleFactor, bool isPath = false)
    {
        var highlight = TerrainHighlight.Instantiate<TerrainHighlight>();
        highlight.Color = color;
        highlight.ScaleFactor = scaleFactor;
        highlight.Position = position;
        AddChild(highlight);

        if (isPath)
        {
            _path.Add(highlight);
        }
    }

    public void Clear()
    {
        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        _path.Clear();
    }
}
