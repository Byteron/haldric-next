using RelEcs;
using RelEcs.Godot;

public class PlateauArea
{
    public float SolidFactor { get; set; }
    public float BlendFactor => 1f - SolidFactor;

    public PlateauArea() => SolidFactor = 0.75f;
    public PlateauArea(float solidFactor)
    {
        SolidFactor = solidFactor;
    }
}