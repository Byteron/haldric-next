using Godot;

public class Metrics
{
    public const float OuterToInner = 0.866025404f;
    public const float InnerToOuter = 1f / OuterToInner;
    public const float OuterRadius = 5f;
    public const float InnerRadius = OuterRadius * OuterToInner;

    public const float ElevationStep = 1.5f;

    public const float KeepOffset = 0.5f;

    public const float NoiseScale = 1f;
    public static Image Noise = GD.Load<Texture2D>("res://assets/graphics/images/noise.png").GetImage();

    public const float CellPerturbStrength = 1f;

    public static readonly Vector3[] Corners = {
        new(-InnerRadius, 0f, 0.5f * OuterRadius), // E
        new(-InnerRadius, 0f, -0.5f * OuterRadius), // SE
        new(0f, 0f, -OuterRadius), // SW
        new(InnerRadius, 0f, -0.5f * OuterRadius), // W
        new(InnerRadius, 0f, 0.5f * OuterRadius), // NW
        new(0f, 0f, OuterRadius) // NE
    };

    public static Color SampleNoise(Vector3 position)
    {
        var x = (int)(position.X * NoiseScale) % 512;
        var z = (int)(position.Z * NoiseScale) % 512;
        return Noise.GetPixel(Mathf.Abs(x), Mathf.Abs(z));
    }

    public static Vector3 Perturb(Vector3 position)
    {
        var sample = SampleNoise(position);

        position.X += (sample.R * 2f - 1f) * CellPerturbStrength;
        position.Z += (sample.G * 2f - 1f) * CellPerturbStrength;

        return position;
    }

    public static Vector3 GetBridge(Direction direction, float blendFactor)
    {
        return (Corners[(int)direction] + Corners[(int)direction.Next()]) * blendFactor;
    }

    public static Vector3 GetFirstCorner(Direction direction)
    {
        return Corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(Direction direction)
    {
        return Corners[(int)direction.Next()];
    }

    public static Vector3 GetFirstSolidCorner(Direction direction, float solidFactor)
    {
        return Corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(Direction direction, float solidFactor)
    {
        return Corners[(int)direction.Next()] * solidFactor;
    }

    public static Vector3 GetEdgeMiddle(Direction direction)
    {
        return (GetFirstCorner(direction) + GetSecondCorner(direction)) * 0.5f;
    }

    public static Vector3 GetSolidEdgeMiddle(Direction direction, float solidFactor)
    {
        return (GetFirstSolidCorner(direction, solidFactor) + GetSecondSolidCorner(direction, solidFactor)) * 0.5f;
    }
}