using System.Collections.Generic;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        Open("res://assets/graphics/models/");

        NewTerrain("Gg", new List<TerrainType>() { TerrainType.Flat });
        NewCastle("Ch", new List<TerrainType>() { TerrainType.Flat, TerrainType.Fortified });
        NewKeep("Kh", new List<TerrainType>() { TerrainType.Flat, TerrainType.Fortified });
        NewTerrain("Ff", new List<TerrainType>() { TerrainType.Flat, TerrainType.Forested });
        NewTerrain("Vh", new List<TerrainType>() { TerrainType.Flat, TerrainType.Settled });
        NewTerrain("Ww", new List<TerrainType>() { TerrainType.Aqueous });

        AddKeepPlateauGraphic("Kh", "keep_plateau");
        AddWallSegmentGraphic("Kh", "keep_wall");
        AddWallTowerGraphic("Kh", "keep_tower");

        AddWallSegmentGraphic("Ch", "castle_wall");
        AddWallTowerGraphic("Ch", "castle_tower");

        AddDecorationGraphic("Ff", "forest");
        AddDecorationGraphic("Vh", "village");
        AddDecorationGraphic("Vh", "village_main");
        AddWaterGraphic("Ww", "water");
    }
}