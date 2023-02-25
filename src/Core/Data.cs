using System;
using System.Collections.Generic;
using Godot;

namespace Haldric;

public partial class Data : Node
{
    public static Data Instance { get; private set; } = default!;

    public Dictionary<string, Terrain> Terrains = new();

    public readonly Dictionary<string, MapData> Maps = new();

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

    public readonly Dictionary<string, int> TextureArrayIds = new();

    public Texture2DArray TextureArray = new();
    public Texture2DArray NormalTextureArray = new();
    public Texture2DArray RoughnessTextureArray = new();

    public Material TerrainMaterial = default!;

    public override void _Ready()
    {
        Instance = this;
        LoadMaps();
        LoadTerrains();
        LoadTerrainGraphics();
    }

    public void LoadMaps()
    {
        foreach (var data in Loader.LoadDir("res://data/maps", new List<string> { "json" }, false))
        {
            var mapData = Loader.LoadJson<MapData>(data.Path);
            Maps.Add(data.Id, mapData);
        }
    }

    public void LoadTerrains()
    {
        Terrains = Loader.LoadJson<Dictionary<string, Terrain>>("res://data/terrain.json")!;
    }

    public void LoadTerrainGraphics()
    {
        var terrainScript = new TerrainGraphicScript();
        terrainScript.Load();

        Decorations = terrainScript.Decorations;
        DirectionalDecorations = terrainScript.DirectionalDecorations;
        WaterGraphics = terrainScript.WaterGraphics;
        WallSegments = terrainScript.WallSegments;
        WallTowers = terrainScript.WallTowers;
        OuterCliffs = terrainScript.OuterCliffs;
        InnerCliffs = terrainScript.InnerCliffs;
        KeepPlateaus = terrainScript.KeepPlateaus;
        TerrainTextures = terrainScript.TerrainTextures;
        TerrainNormalTextures = terrainScript.TerrainNormalTextures;
        TerrainRoughnessTextures = terrainScript.TerrainRoughnessTextures;

        var terrainCount = 0;
        foreach (var (code, terrain) in Terrains)
        {
            terrain.Index = terrainCount;
            TextureArrayIds.Add(code, terrainCount);
            terrainCount += 1;
        }

        TextureArray = CreateTextureArray(terrainCount, TextureArray, TerrainTextures);
        NormalTextureArray = CreateTextureArray(terrainCount, NormalTextureArray, TerrainNormalTextures);
        RoughnessTextureArray = CreateTextureArray(terrainCount, RoughnessTextureArray, TerrainRoughnessTextures);

        TerrainMaterial = ResourceLoader.Load<Material>("res://assets/graphics/materials/terrain.tres");

        TerrainMaterial.Set("shader_parameter/textures", TextureArray);
        TerrainMaterial.Set("shader_parameter/normal_textures", NormalTextureArray);
        TerrainMaterial.Set("shader_parameter/roughness_textures", RoughnessTextureArray);
    }

    Texture2DArray CreateTextureArray(int terrainCount, Texture2DArray texArray,
        Dictionary<string, Texture2D> textureDict)
    {
        var textures = new Godot.Collections.Array<Image>();
        textures.Resize(terrainCount);

        for (var i = 0; i < textures.Count; i++)
        {
            var tex = textureDict["Gg"];
            textures[i] = tex.GetImage();
        }

        foreach (var (code, texture) in textureDict)
        {
            var index = TextureArrayIds[code];

            var image = texture.GetImage();
            textures[index] = image;
        }

        texArray._Images = textures;

        return texArray;
    }
}