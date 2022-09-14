using Godot;
using RelEcs;

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
        return coords.ToOffset().x > -1 && coords.ToOffset().x < Width
            && coords.ToOffset().z > -1 && coords.ToOffset().z < Height;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Width, Height);
    }
}