using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;

public partial class Data : Node
{
    public static Data Instance { get; private set; }

    public EcsWorld World;

    public Dictionary<string, EcsEntity> Terrains = new Dictionary<string, EcsEntity>();
    public Dictionary<string, List<TerrainGraphic>> Decorations = new Dictionary<string, List<TerrainGraphic>>();
    public Dictionary<string, TerrainGraphic> WaterGraphics = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> KeepPlateaus = new Dictionary<string, TerrainGraphic>();

    public override void _Ready()
    {
        Instance = this;
    }

    public void Scan()
    {
        LoadTerrain();
    }

    private void LoadTerrain()
    {
        Terrains.Clear();
        Decorations.Clear();
        WaterGraphics.Clear();
        WallSegments.Clear();
        WallTowers.Clear();
        KeepPlateaus.Clear();

        var terrainScript = new TerrainScript();

        GD.Print(terrainScript);

        terrainScript.World = World;
        terrainScript.Load();

        Terrains = terrainScript.Terrains;
        Decorations = terrainScript.Decorations;
        WaterGraphics = terrainScript.WaterGraphics;
        WallSegments = terrainScript.WallSegments;
        WallTowers = terrainScript.WallTowers;
        KeepPlateaus = terrainScript.KeepPlateaus;
    }
}
