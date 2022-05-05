using Godot;
using RelEcs;
using RelEcs.Godot;

public struct Grid : IReset<Grid>
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void Reset(ref Grid c)
    {
        c.Width = 0;
        c.Height = 0;
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