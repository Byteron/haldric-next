using Godot;
namespace Haldric;

public enum Direction
{
    E,
    Se,
    Sw,
    W,
    Nw,
    Ne
}

public static class DirectionExtensions
{
    static readonly float[] Rotations =
    {
        Mathf.Pi,
        2 * Mathf.Pi / 3,
        Mathf.Pi / 3,
        0,
        5 * Mathf.Pi / 3,
        4 * Mathf.Pi / 3
    };

    public static float Rotation(this Direction direction)
    {
        return Rotations[(int)direction];
    }

    public static Direction Opposite(this Direction direction)
    {
        return (int)direction < 3 ? direction + 3 : direction - 3;
    }

    public static Direction Previous(this Direction direction)
    {
        return direction == 0 ? (Direction)5 : direction - 1;
    }

    public static Direction Next(this Direction direction)
    {
        return direction == (Direction)5 ? 0 : direction + 1;
    }

    public static Direction Previous2(this Direction direction)
    {
        direction -= 2;
        return (int)direction <= 0 ? direction : direction + 6;
    }

    public static Direction Next2(this Direction direction)
    {
        direction += 2;
        return (int)direction >= 5 ? direction : direction - 6;
    }
}