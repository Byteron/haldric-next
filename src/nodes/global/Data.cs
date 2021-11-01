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

    public Dictionary<string, PackedScene> Schedules { get; set; } = new Dictionary<string, PackedScene>();
    public Dictionary<string, PackedScene> Units { get; set; } = new Dictionary<string, PackedScene>();
    public Dictionary<string, Faction> Factions { get; set; }= new Dictionary<string, Faction>();
    public Dictionary<string, Dictionary<string, object>> TerrainDicts { get; set; } = new Dictionary<string, Dictionary<string, object>>();
    public Dictionary<string, EcsEntity> Terrains { get; set; } = new Dictionary<string, EcsEntity>();
    public Dictionary<string, Godot.Collections.Dictionary> Maps { get; set; } = new Dictionary<string, Godot.Collections.Dictionary>();

    public Dictionary<string, Dictionary<string, TerrainGraphic>> Decorations { get; set; } = new Dictionary<string, Dictionary<string, TerrainGraphic>>();
    public Dictionary<string, Dictionary<string, TerrainGraphic>> DirectionalDecorations { get; set; } = new Dictionary<string, Dictionary<string, TerrainGraphic>>();
    public Dictionary<string, TerrainGraphic> WaterGraphics { get; set; } = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments { get; set; } = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers { get; set; } = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus { get; set; } = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, Texture2D> TerrainTextures { get; set; } = new Dictionary<string, Texture2D>();

    public Dictionary<string, int> TextureArrayIds { get; set; } = new Dictionary<string, int>();
    public Texture2DArray TextureArray { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void LoadFactions()
    {
        Factions.Clear();
        
        var spears = new Faction();
        spears.Name = "Spears";
        spears.Recruits = new List<string>() { "Cavalryman", "Spearman", "Bowman" };
        spears.Leaders = new List<string>() { "Spearman" };

        var bows = new Faction();
        bows.Name = "Bows";
        bows.Recruits = new List<string>() { "Cavalryman", "Spearman", "Bowman", "Orcish Archer" };
        bows.Leaders = new List<string>() { "Bowman" };
        
        Factions.Add(spears.Name, spears);
        Factions.Add(bows.Name, bows);
    }

    public void LoadUnits()
    {
        Units.Clear();

        foreach(var data in Loader.LoadDir("res://data/units", new List<string>() {"tscn"}))
        {
            Units.Add(data.Id, (PackedScene)data.Data);
        }
    }

    public void LoadSchedules()
    {
        Schedules.Clear();

        foreach(var data in Loader.LoadDir("res://data/schedules", new List<string>() {"tscn"}))
        {
            Schedules.Add(data.Id, (PackedScene)data.Data);
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
        DirectionalDecorations = terrainScript.DirectionalDecorations;
        WaterGraphics = terrainScript.WaterGraphics;
        WallSegments = terrainScript.WallSegments;
        WallTowers = terrainScript.WallTowers;
        KeepPlateaus = terrainScript.KeepPlateaus;
        TerrainTextures = terrainScript.TerrainTextures;

        TextureArray = CreateTextureArray();
    }

    public void LoadMaps()
    {
        Maps.Clear();

        foreach(var data in Loader.LoadDir("res://data/maps", new List<string>() {"json"}, false))
        {
            var dict = LoadJsonFromFile(data.Path);
            Maps.Add(data.Id, dict);
        }
    }

    private Godot.Collections.Dictionary LoadJsonFromFile(string path)
    {
        var file = new File();
        
        if (!(file.Open(path, File.ModeFlags.Read) == Error.Ok))
        {
            GD.PushError("error reading file");
            return new Godot.Collections.Dictionary();
        }

        var jsonString = file.GetAsText();

        var json = new JSON();
        json.Parse(jsonString);
        return json.GetData() as Godot.Collections.Dictionary;
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
            
            var image = terrainTexture.GetImage();
            GD.Print(image.GetFormat().ToString());
            textures.Add(image);
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
