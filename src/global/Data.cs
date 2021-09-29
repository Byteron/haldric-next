using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public partial class Data : Node
{
    public static Data Instance { get; private set; }

    public Dictionary<string, Godot.Collections.Dictionary> UnitDicts = new Dictionary<string, Godot.Collections.Dictionary>();
    public Dictionary<string, Dictionary<string, object>> TerrainDicts = new Dictionary<string, Dictionary<string, object>>();
    public Dictionary<string, EcsEntity> Terrains = new Dictionary<string, EcsEntity>();

    public Dictionary<string, List<TerrainGraphic>> Decorations = new Dictionary<string, List<TerrainGraphic>>();
    public Dictionary<string, TerrainGraphic> WaterGraphics = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, Texture2D> TerrainTextures = new Dictionary<string, Texture2D>();

    public Dictionary<string, uint> TextureArrayIds = new Dictionary<string, uint>();
    public Texture2DArray TextureArray { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void Scan()
    {
        LoadUnits();
        LoadTerrain();
    }

    private void LoadUnits()
    {
        UnitDicts.Clear();

        foreach(var data in Loader.LoadDir("res://data/units", new List<string>() {"json"}, false))
        {
            var file = new File();
            file.Open(data.Path, File.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var json = new JSON();
            json.Parse(jsonString);
            var dict = json.GetData() as Godot.Collections.Dictionary;
            UnitDicts.Add((string)dict["id"], dict);
        }

        GD.Print(UnitDicts);
    }

    private void LoadTerrain()
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
            textures.Add(item.Value.GetImage());
            TextureArrayIds.Add(item.Key, (uint)index);
            index += 1;
        }

        texArray._Images = textures;

		return texArray;
	}

    public EcsEntity CreateUnit(string unitType)
    {
        var dict = UnitDicts[unitType];
        return UnitFactory.CreateFromDict(dict);
    }

    public EcsEntity CreateTerrain(string terrainType)
    {
        var dict = TerrainDicts[terrainType];
        return TerrainFactory.CreateFromDict(dict);
    }
}
