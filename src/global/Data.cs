using Godot;
using System;
using System.Collections.Generic;

public partial class Data : Node
{
    public static Data Instance { get; private set; }
    
    public Dictionary<string, Terrain> Terrains = new Dictionary<string, Terrain>();
    public Dictionary<string, TerrainGraphic> Decorations = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallSegments = new Dictionary<string, TerrainGraphic>();
    public Dictionary<string, TerrainGraphic> WallTowers = new Dictionary<string, TerrainGraphic>();
    
    public override void _Ready()
    {
        Instance = this;

        Scan();
    }

    public void Scan()
    {
        LoadTerrain();
    }

    private void LoadTerrain()
    {
        Terrains.Clear();
        Decorations.Clear();
        WallSegments.Clear();
        WallTowers.Clear();

        var terrainScript = new TerrainScript();

        GD.Print(terrainScript);

        terrainScript.Load();

        Terrains = terrainScript.Terrains;
        Decorations = terrainScript.Decorations;
        WallSegments = terrainScript.WallSegments;
        WallTowers = terrainScript.WallTowers;

        GD.Print(Terrains);
    }
}
