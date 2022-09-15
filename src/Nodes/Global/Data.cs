using Godot;
using System.Collections.Generic;
using RelEcs;

public partial class Data : Node
{
    public static Data Instance { get; private set; }

    public Color[] SideColors =
    {
        new("FF0000"),
        new("0000FF"),
        new("00FF00"),
        new("FFFF00"),
        new("00FFFF"),
        new("FF00FF"),
        new("000000"),
        new("FFFFFF"),
    };

    public Dictionary<string, PackedScene> Schedules = new();
    public Dictionary<string, PackedScene> Units = new();
    public Dictionary<string, FactionData> Factions = new();
    public Dictionary<string, Dictionary<string, object>> TerrainDicts = new();
    public Dictionary<string, Entity> Terrains = new();
    public Dictionary<string, MapData> Maps = new();

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

    public override void _Ready()
    {
        Instance = this;
    }

    public void LoadFactions(World world)
    {
        Factions.Clear();

        foreach (var data in Loader.LoadDir("res://data/factions", new List<string>() { "tres" }))
        {
            var faction = (FactionData)data.Data;
            Factions.Add(faction.Name, faction);
        }
    }

    public void LoadUnits(World world)
    {
        Units.Clear();

        foreach (var data in Loader.LoadDir("res://data/units", new List<string>() { "tscn" }))
        {
            Units.Add(data.Id, (PackedScene)data.Data);
        }
    }

    public void LoadSchedules(World world)
    {
        Schedules.Clear();

        foreach (var data in Loader.LoadDir("res://data/schedules", new List<string>() { "tscn" }))
        {
            Schedules.Add(data.Id, (PackedScene)data.Data);
        }
    }

    public void LoadTerrain(World world)
    {
        TerrainDicts.Clear();
        Decorations.Clear();
        WaterGraphics.Clear();
        WallSegments.Clear();
        WallTowers.Clear();
        KeepPlateaus.Clear();
        TerrainTextures.Clear();

        var terrainScript = new TerrainScript();

        terrainScript.Load();

        TerrainDicts = terrainScript.TerrainDicts;

        foreach (var pair in TerrainDicts)
        {
            Terrains.Add(pair.Key, TerrainFactory.CreateFromDict(world, pair.Value));
        }

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
        DefaultOverlayBaseTerrains = terrainScript.DefaultOverlayBaseTerrains;

        CreateTextureArrayIds(world);

        TextureArray = CreateTextureArray(TextureArray, TerrainTextures);
        NormalTextureArray = CreateTextureArray(NormalTextureArray, TerrainNormalTextures);
        RoughnessTextureArray = CreateTextureArray(RoughnessTextureArray, TerrainRoughnessTextures);
    }

    public void LoadMaps(World world)
    {
        Maps.Clear();

        foreach (var data in Loader.LoadDir("res://data/maps", new List<string>() { "json" }, false))
        {
            var mapData = Loader.LoadJson<MapData>(data.Path);
            Maps.Add(data.Id, mapData);
        }
    }

    public void CreateTextureArrayIds(World world)
    {
        var index = 0;
        foreach (var item in Terrains)
        {
            var terrainCode = item.Key;
            var terrainEntity = item.Value;

            world.AddComponent(terrainEntity.Identity, new TerrainTypeIndex { Value = index });

            TextureArrayIds.Add(terrainCode, index);
            index += 1;
        }
    }

    public Texture2DArray CreateTextureArray(Texture2DArray texArray, Dictionary<string, Texture2D> textureDict)
    {
        var textures = new Godot.Collections.Array<Image>();
        textures.Resize(Terrains.Count);

        for (int i = 0; i < textures.Count; i++)
        {
            textures[i] = textureDict["Gg"].GetImage();
        }

        foreach (var item in textureDict)
        {
            var terrainCode = item.Key;
            var terrainTexture = item.Value;

            var index = TextureArrayIds[terrainCode];

            var image = terrainTexture.GetImage();
            textures[index] = image;
        }

        texArray._Images = textures;

        return texArray;
    }
}