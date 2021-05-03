using Godot;
using Leopotam.Ecs;

public struct Grid : IEcsAutoReset<Grid>
{
    public int Width;
    public int Height;

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void AutoReset(ref Grid c)
    {
        c.Width = 0;
        c.Height = 0;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Width, Height);
    }
}