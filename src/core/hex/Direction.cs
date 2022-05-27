
public enum Direction { E, Se, Sw, W, Nw, Ne }

public static class DirectionExtentions
{
     static float[] rotations = new float[] {
        Godot.Mathf.Pi,
        2 * Godot.Mathf.Pi / 3,
        Godot.Mathf.Pi / 3,
        0,
        5 * Godot.Mathf.Pi / 3,
        4 * Godot.Mathf.Pi / 3
    };

    public static float Rotation(this Direction direction)
    {
        return rotations[(int)direction];
    }
    public static Direction Opposite(this Direction direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static Direction Previous(this Direction direction)
    {
        return direction == (Direction)0 ? (Direction)5 : (direction - 1);
    }

    public static Direction Next(this Direction direction)
    {
        return direction == (Direction)5 ? (Direction)0 : (direction + 1);
    }

    public static Direction Previous2(this Direction direction)
    {
        direction -= 2;
        return (int)direction <= 0 ? direction : (direction + 6);
    }

    public static Direction Next2(this Direction direction)
    {
        direction += 2;
        return (int)direction >= 5 ? direction : (direction - 6);
    }
}