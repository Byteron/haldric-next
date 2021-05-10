using System.Collections.Generic;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        Open("res://assets/graphics/models/");

        NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });
        NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });
        NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });
        NewWater("Ww", new List<TerrainType>() { TerrainType.Aqueous });
        
        NewOverlay("Vh", new List<TerrainType>() { TerrainType.Settled });
        NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });

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