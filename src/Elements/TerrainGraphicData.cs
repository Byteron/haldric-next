using System.Collections.Generic;
using Godot;

public class TerrainGraphic
{
    public string Code = string.Empty;
    public Mesh Mesh;
    public Vector3 Offset;
    public List<Mesh> Variations = new();

    public void AddVariation(Mesh mesh)
    {
        if (Variations.Count == 0)
        {
            Variations.Add(Mesh);
        }

        Variations.Add(mesh);
    }
}

public class TerrainGraphicData
{
    public Dictionary<string, Dictionary<string, TerrainGraphic>> Decorations = new();
    public Dictionary<string, Dictionary<string, TerrainGraphic>> DirectionalDecorations = new();
    public Dictionary<string, TerrainGraphic> WaterGraphics = new();
    public Dictionary<string, TerrainGraphic> WallSegments = new();
    public Dictionary<string, TerrainGraphic> WallTowers = new();
    public Dictionary<string, Dictionary<string, TerrainGraphic>> OuterCliffs = new();
    public Dictionary<string, Dictionary<string, TerrainGraphic>> InnerCliffs = new();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new();
    public Dictionary<string, Texture2D> TerrainTextures = new();
    public Dictionary<string, Texture2D> TerrainNormalTextures = new();
    public Dictionary<string, Texture2D> TerrainRoughnessTextures = new();
    public Dictionary<string, string> DefaultOverlayBaseTerrains = new();

    public Dictionary<string, int> TextureArrayIds = new();

    public Texture2DArray TextureArray = new();
    public Texture2DArray NormalTextureArray = new();
    public Texture2DArray RoughnessTextureArray = new();
}