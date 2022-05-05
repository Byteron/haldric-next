using RelEcs;
using RelEcs.Godot;

public struct PlateauArea : IReset<PlateauArea>
{
    public float SolidFactor { get; set; }
    public float BlendFactor { get { return 1f - SolidFactor; } }

    public PlateauArea(float solidFactor)
    {
        SolidFactor = solidFactor;
    }

    public void Reset(ref PlateauArea c)
    {
        c.SolidFactor = 0f;
    }
}