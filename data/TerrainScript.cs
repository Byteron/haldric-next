using System.Collections.Generic;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        Open("res://assets/graphics/models/");

        NewTerrain("Gg", new List<TerrainType>() { TerrainType.Flat });
        NewTerrain("Ch", new List<TerrainType>() { TerrainType.Flat, TerrainType.Fortified });
        NewTerrain("Kh", new List<TerrainType>() { TerrainType.Flat, TerrainType.Fortified });
        NewTerrain("Ff", new List<TerrainType>() { TerrainType.Flat, TerrainType.Forested });
        NewTerrain("Mm", null);

        NewKeepPlateau("Kh", "keep_plateau");
        NewWallSegment("Kh", "keep_wall");
        NewWallTower("Kh", "keep_tower");

        NewWallSegment("Ch", "castle_wall");
        NewWallTower("Ch", "castle_tower");

        NewDecoration("Ff", "forest");
    }
}