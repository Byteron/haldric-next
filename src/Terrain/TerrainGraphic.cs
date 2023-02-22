using System.Collections.Generic;
using Godot;

public class TerrainGraphic
{
    public string Code = string.Empty;
    public Mesh Mesh;
    public Vector3 Offset;
    public readonly List<Mesh> Variations = new();

    public void AddVariation(Mesh mesh)
    {
        if (Variations.Count == 0)
        {
            Variations.Add(Mesh);
        }

        Variations.Add(mesh);
    }
}