using System.Collections.Generic;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });
        NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });
        NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });
        NewWater("Ww", new List<TerrainType>() { TerrainType.Aqueous });
        
        NewOverlay("Vh", new List<TerrainType>() { TerrainType.Settled });
        NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });

        AddTerrainTexture("Gg", "assets/graphics/images/grass.png");
        AddTerrainTexture("Ch", "assets/graphics/images/stone.png");
        AddTerrainTexture("Kh", "assets/graphics/images/stone.png");
        AddTerrainTexture("Ww", "assets/graphics/images/mud.png");

        AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres");
        AddWallSegmentGraphic("Kh", "assets/graphics/models/keep_wall.tres");
        AddWallTowerGraphic("Kh", "assets/graphics/models/keep_tower.tres");

        AddWallSegmentGraphic("Ch", "assets/graphics/models/castle_wall.tres");
        AddWallTowerGraphic("Ch", "assets/graphics/models/castle_tower.tres");

        AddDecorationGraphic("Ff", "assets/graphics/models/forest.tres");
        AddDecorationGraphic("Vh", "assets/graphics/models/village.tres");
        AddDecorationGraphic("Vh", "assets/graphics/models/village_main.tres");
        AddWaterGraphic("Ww", "assets/graphics/models/water.tres");
    }
}