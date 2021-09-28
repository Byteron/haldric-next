using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public abstract class TerrainLoader
{
    public Dictionary<string, EcsEntity> Terrains = new Dictionary<string, EcsEntity>();
    public Dictionary<string, List<TerrainGraphic>> Decorations = new Dictionary<string, List<TerrainGraphic>>();
    public Dictionary<string, TerrainGraphic> WaterGraphics = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, Texture2D> TerrainTextures = new Dictionary<string, Texture2D>();

    private TerrainBuilder _terrainBuilder = new TerrainBuilder();
    
    private TerrainGraphicBuilder _terrainGraphicBuilder = new TerrainGraphicBuilder();

    public abstract void Load();

    public void NewBase(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).Build();
        Terrains.Add(code, terrain);
    }

    public void NewWater(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithHasWater().Build();
        Terrains.Add(code, terrain);
    }

    public void NewCastle(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithRecruitTo().Build();
        Terrains.Add(code, terrain);
    }

    public void NewKeep(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.CreateBase().WithCode(code).WithTypes(types).WithRecruitFrom().WithRecruitTo().Build();
        Terrains.Add(code, terrain);
    }

    public void NewOverlay(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.CreateOverlay().WithCode(code).WithTypes(types).Build();
        Terrains.Add(code, terrain);
    }

    public void AddTerrainTexture(string code, string path)
    {
        TerrainTextures.Add(code, LoadAsset<Texture2D>(path));
    }

    public void AddDecorationGraphic(string code, string path)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();

        if (!Decorations.ContainsKey(code))
        {
            Decorations.Add(code, new List<TerrainGraphic>());
        }

        var list = Decorations[code];
        list.Add(graphic);
    }

    public void AddWaterGraphic(string code, string path)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();

        WaterGraphics.Add(code, graphic);
    }

    public void AddWallSegmentGraphic(string code, string path)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
        WallSegments.Add(code, graphic);
    }

    public void AddWallTowerGraphic(string code, string path)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
        WallTowers.Add(code, graphic);
    }

    public void AddKeepPlateauGraphic(string code, string path)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(LoadAsset<Mesh>(path)).Build();
        KeepPlateaus.Add(code, graphic);
    }

    private T LoadAsset<T>(string path) where T: RefCounted
    {
        return GD.Load<T>("res://" + path);
    }
}