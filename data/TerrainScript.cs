using System.Collections.Generic;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        Open("res://assets/graphics/models/");

        NewTerrain("Grass", "Gg", new List<TerrainType>() { TerrainType.Flat });
        NewTerrain("Castle", "Ch", new List<TerrainType>() { TerrainType.Flat, TerrainType.Fortified });
        NewTerrain("Keep", "Kh", new List<TerrainType>() { TerrainType.Flat, TerrainType.Fortified });
        NewTerrain("Forest", "Ff", new List<TerrainType>() { TerrainType.Flat, TerrainType.Forested });
        NewTerrain("Mountains", "Mm", null);

        NewKeepPlateau("Kh", "keep_plateau");
        NewWallSegment("Kh", "keep_wall");
        NewWallTower("Kh", "keep_tower");

        NewWallSegment("Ch", "castle_wall");
        NewWallTower("Ch", "castle_tower");

        NewDecoration("Ff", "forest");
    }
}