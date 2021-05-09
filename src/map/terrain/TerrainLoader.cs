using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;

public abstract class TerrainLoader
{
    public EcsWorld World;

    public Dictionary<string, EcsEntity> Terrains = new Dictionary<string, EcsEntity>();
    public Dictionary<string, List<TerrainGraphic>> Decorations = new Dictionary<string, List<TerrainGraphic>>();
    public Dictionary<string, TerrainGraphic> WaterGraphics = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();

    private string _root = "res://";

    private Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();

    private TerrainBuilder _terrainBuilder;
    
    private TerrainGraphicBuilder _terrainGraphicBuilder = new TerrainGraphicBuilder();

    public abstract void Load();

    public void Open(string path)
    {
        _terrainBuilder = new TerrainBuilder(World);

        _meshes.Clear();
        _root = path;

        foreach (var fileData in Loader.LoadDir(path, new List<string>() { "tres", "res", "mesh", "obj" }))
        {
            var semiPath = fileData.Path.BaseName().Replace(_root, "");

            if (semiPath.BeginsWith("/"))
            {
                semiPath.Remove(0, 1);
            }

            _meshes.Add(semiPath, fileData.Data as Mesh);
        }
    }

    public void NewTerrain(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.Create().WithCode(code).WithTypes(types).Build();
        Terrains.Add(code, terrain);
    }

    public void NewCastle(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.Create().WithCode(code).WithTypes(types).WithRecruitTo().Build();
        Terrains.Add(code, terrain);
    }

    public void NewKeep(string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.Create().WithCode(code).WithTypes(types).WithRecruitFrom().WithRecruitTo().Build();
        Terrains.Add(code, terrain);
    }

    public void AddDecorationGraphic(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();

        if (!Decorations.ContainsKey(code))
        {
            Decorations.Add(code, new List<TerrainGraphic>());
        }

        var list = Decorations[code];
        list.Add(graphic);
    }

    public void AddWaterGraphic(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();

        WaterGraphics.Add(code, graphic);
    }

    public void AddWallSegmentGraphic(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        WallSegments.Add(code, graphic);
    }

    public void AddWallTowerGraphic(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        WallTowers.Add(code, graphic);
    }

    public void AddKeepPlateauGraphic(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        KeepPlateaus.Add(code, graphic);
    }
}