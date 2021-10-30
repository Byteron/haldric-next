using Godot;

public class Metrics
{
    public const float OuterToInner = 0.866025404f;
    public const float InnerToOuter = 1f / OuterToInner;
    public const float OuterRadius = 5f;
    public const float InnerRadius = OuterRadius * OuterToInner;

    public const float ElevationStep = 1.5f;
    public const int DeepWaterOffset = -9;
    public const int ShallowWaterOffset = -3;
    
    public const float NoiseScale = 1f;
    public static Image Noise = GD.Load<Texture2D>("res://assets/graphics/images/noise.png").GetImage();

    public const float CellPerturbStrength = 1f;

    public static readonly Vector3[] corners = {
        new Vector3(0f, 0f, OuterRadius),
        new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
        new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(0f, 0f, -OuterRadius),
        new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius),
    };

    public static Color SampleNoise(Vector3 position)
    {
        int x = (int)(position.x * NoiseScale) % 512;
        int z = (int)(position.z * NoiseScale) % 512;
        return Noise.GetPixel(Mathf.Abs(x), Mathf.Abs(z));
    }

    public static Vector3 Perturb(Vector3 position)
    {
        Color sample = SampleNoise(position);

        position.x += (sample.r * 2f - 1f) * CellPerturbStrength;
        position.z += (sample.g * 2f - 1f) * CellPerturbStrength;

        return position;
    }

    public static Vector3 GetBridge(Direction direction, PlateauArea plateauArea)
    {
        return (corners[(int)direction] + corners[(int)direction.Next()]) * plateauArea.BlendFactor;
    }

    public static Vector3 GetFirstCorner(Direction direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(Direction direction)
    {
        return corners[(int)direction.Next()];
    }

    public static Vector3 GetFirstSolidCorner(Direction direction, PlateauArea plateauArea)
    {
        return corners[(int)direction] * plateauArea.SolidFactor;
    }

    public static Vector3 GetSecondSolidCorner(Direction direction, PlateauArea plateauArea)
    {
        return corners[(int)direction.Next()] * plateauArea.SolidFactor;
    }

    public static Vector3 GetEdgeMiddle(Direction direction)
    {
        return (GetFirstCorner(direction) + GetSecondCorner(direction)) * 0.5f;
    }

    public static Vector3 GetSolidEdgeMiddle(Direction direction, PlateauArea plateauArea)
    {
        return (GetFirstSolidCorner(direction, plateauArea) + GetSecondSolidCorner(direction, plateauArea)) * 0.5f;
    }
}