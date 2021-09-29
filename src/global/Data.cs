using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public partial class Data : Node
{
    public static Data Instance { get; private set; }

    public Dictionary<string, Godot.Collections.Dictionary> Units = new Dictionary<string, Godot.Collections.Dictionary>();
    public Dictionary<string, Godot.Collections.Dictionary> Terrains = new Dictionary<string, Godot.Collections.Dictionary>();

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
        Units.Clear();

        foreach(var data in Loader.LoadDir("res://data/units", new List<string>() {"json"}, false))
        {
            var file = new File();
            file.Open(data.Path, File.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var json = new JSON();
            json.Parse(jsonString);
            var dict = json.GetData() as Godot.Collections.Dictionary;
            Units.Add((string)dict["id"], dict);
        }

        GD.Print(Units);
    }

    private void LoadTerrain()
    {
        Terrains.Clear();
        Decorations.Clear();
        WaterGraphics.Clear();
        WallSegments.Clear();
        WallTowers.Clear();
        KeepPlateaus.Clear();
        TerrainTextures.Clear();

        var terrainScript = new TerrainScript();

        terrainScript.Load();

        Terrains = terrainScript.Terrains;
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
}
