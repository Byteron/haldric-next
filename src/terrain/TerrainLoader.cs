using Godot;
using System.Collections.Generic;

public abstract class TerrainLoader
{
    public Dictionary<string, TerrainData> Terrains = new Dictionary<string, TerrainData>();
    public Dictionary<string, TerrainGraphic> Decorations = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();

    private string _root = "res://";

    private Dictionary<string, Mesh> _meshes = new Dictionary<string, Mesh>();

    private TerrainBuilder _terrainBuilder = new TerrainBuilder();
    private TerrainGraphicBuilder _terrainGraphicBuilder = new TerrainGraphicBuilder();

    public abstract void Load();

    public void Open(string path)
    {
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

    public void NewTerrain(string name, string code, List<TerrainType> types)
    {
        var terrain = _terrainBuilder.Create().WithName(name).WithCode(code).WithTypes(types).Build();
        Terrains.Add(code, terrain);
    }

    public void NewDecoration(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        Decorations.Add(code, graphic);
    }

    public void NewWallSegment(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        WallSegments.Add(code, graphic);
    }

    public void NewWallTower(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        WallTowers.Add(code, graphic);
    }

    public void NewKeepPlateau(string code, string image_stem)
    {
        var graphic = _terrainGraphicBuilder.Create().WithCode(code).WithMesh(_meshes[image_stem]).Build();
        KeepPlateaus.Add(code, graphic);
    }
}