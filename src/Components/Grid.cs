using Godot;
using RelEcs;

public class Grid
{
    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public bool IsCoordsInGrid(Coords coords)
    {
        return coords.ToOffset().X > -1 && coords.ToOffset().X < Width
            && coords.ToOffset().Z > -1 && coords.ToOffset().Z < Height;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Width, Height);
    }
}