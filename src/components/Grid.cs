using Godot;
using RelEcs;
using RelEcs.Godot;

public class Grid
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public bool IsCoordsInGrid(Coords coords)
    {
        return coords.Offset().x > -1 && coords.Offset().x < Width
            && coords.Offset().z > -1 && coords.Offset().z < Height;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Width, Height);
    }
}