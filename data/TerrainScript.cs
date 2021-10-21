using System.Collections.Generic;
using Godot;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });
        
        NewWater("Ww", new List<TerrainType>() { TerrainType.Aqueous });
        
        NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });

        NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });
        NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });

        NewHouses("Vh", new List<TerrainType>() { TerrainType.Settled });
        NewVillage("VMh", new List<TerrainType>() { TerrainType.Settled });

        AddTerrainTexture("Gg", "assets/graphics/images/grass.png");
        AddTerrainTexture("Ch", "assets/graphics/images/stone.png");
        AddTerrainTexture("Kh", "assets/graphics/images/stone.png");
        AddTerrainTexture("Ww", "assets/graphics/images/mud.png");

        AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 1.5f, 0f));
        AddWallSegmentGraphic("Kh", "assets/graphics/models/keep_wall.tres");
        AddWallTowerGraphic("Kh", "assets/graphics/models/keep_tower.tres");

        AddKeepPlateauGraphic("Ch", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 0.1f, 0f));
        AddWallSegmentGraphic("Ch", "assets/graphics/models/castle_wall.tres");
        AddWallTowerGraphic("Ch", "assets/graphics/models/castle_tower.tres");

        AddDecorationGraphic("Ff", "assets/graphics/models/forest_low_poly.tres");
        AddDecorationGraphic("Vh", "assets/graphics/models/village.tres");
        AddDecorationGraphic("VMh", "assets/graphics/models/village_main.tres");
        AddWaterGraphic("Ww", "assets/graphics/models/water.tres");
    }
}