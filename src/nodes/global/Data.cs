using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public partial class Data : Node
{
    public static Data Instance { get; private set; }

    public Color[] TeamColors = new Color[]
    {
        new Color("FF0000"),
        new Color("00FF00"),
        new Color("0000FF"),
        new Color("FFFF00"),
        new Color("00FFFF"),
        new Color("FF00FF"),
        new Color("000000"),
        new Color("FFFFFF"),
    };

    public Dictionary<string, PackedScene> Units = new Dictionary<string, PackedScene>();
    public Dictionary<string, Dictionary<string, object>> TerrainDicts = new Dictionary<string, Dictionary<string, object>>();
    public Dictionary<string, EcsEntity> Terrains = new Dictionary<string, EcsEntity>();

    public Dictionary<string, Dictionary<string, TerrainGraphic>> Decorations = new Dictionary<string, Dictionary<string, TerrainGraphic>>();
    public Dictionary<string, TerrainGraphic> WaterGraphics = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, Texture2D> TerrainTextures = new Dictionary<string, Texture2D>();

    public Dictionary<string, int> TextureArrayIds = new Dictionary<string, int>();
    public Texture2DArray TextureArray { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void LoadUnits()
    {
        Units.Clear();

        foreach(var data in Loader.LoadDir("res://data/units", new List<string>() {"tscn"}))
        {
            Units.Add(data.Id, (PackedScene)data.Data);
        }
    }

    public void LoadTerrain()
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
            Terrains.Add(pair.Key, TerrainFactory.CreateFromDict(pair.Value));
        }

        Decorations = terrainScript.Decorations;
        WaterGraphics = terrainScript.WaterGraphics;
        WallSegments = terrainScript.WallSegments;
        WallTowers = terrainScript.WallTowers;
        KeepPlateaus = terrainScript.KeepPlateaus;
        TerrainTextures = terrainScript.TerrainTextures;

        TextureArray = CreateTextureArray();
    }

    public Texture2DArray CreateTextureArray()
	{
		Texture2DArray texArray = new Texture2DArray();
        
        var textures = new Godot.Collections.Array();
        
        var index = 0;
        foreach (var item in TerrainTextures)
        {
            var terrainCode = item.Key;
            var terrainTexture = item.Value;

            var terrainEntity = Terrains[terrainCode];
            terrainEntity.Add(new TerrainTypeIndex(index));

            textures.Add(terrainTexture.GetImage());
            TextureArrayIds.Add(terrainCode, index);
            
            index += 1;
        }

        texArray._Images = textures;

		return texArray;
	}

    public EcsEntity CreateTerrain(string terrainType)
    {
        var dict = TerrainDicts[terrainType];
        return TerrainFactory.CreateFromDict(dict);
    }
}
