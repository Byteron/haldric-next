using System.Collections.Generic;
using Haldric.Wdk;

public class TerrainScript : TerrainLoader
{
    public override void Load()
    {
        NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });
        NewBase("Rd", new List<TerrainType>() { TerrainType.Flat });
        NewBase("Dd", new List<TerrainType>() { TerrainType.Rough });
        
        NewWater("Ww", new List<TerrainType>() { TerrainType.Aqueous });
        
        NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });

        NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });
        NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });

        NewHouses("Vh", new List<TerrainType>() { TerrainType.Settled });
        NewVillage("VHh", new List<TerrainType>() { TerrainType.Settled });

        AddTerrainTexture("Gg", "assets/graphics/images/grass.png");
        AddTerrainTexture("Dd", "assets/graphics/images/mud.png");
        AddTerrainTexture("Rd", "assets/graphics/images/stone.png");
        AddTerrainTexture("Ch", "assets/graphics/images/stone.png");
        AddTerrainTexture("Kh", "assets/graphics/images/stone.png");
        AddTerrainTexture("Ww", "assets/graphics/images/mud.png");

        AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 1.5f, 0f));
        AddWallSegmentGraphic("Kh", "assets/graphics/models/keep_wall.tres");
        AddWallTowerGraphic("Kh", "assets/graphics/models/keep_tower.tres");

        // AddKeepPlateauGraphic("Ch", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 0.1f, 0f));
        AddWallSegmentGraphic("Ch", "assets/graphics/models/castle_wall.tres");
        AddWallTowerGraphic("Ch", "assets/graphics/models/castle_tower.tres");

        AddDecorationGraphic("Ff", "assets/graphics/models/forest_low_poly.tres");

        AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_center_01-Village_human_houses_01.tres", "center");
        AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_center_02-Village_human_houses_01.tres", "center");
        AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_center_01-Village_human_basefloor_01.tres", "centerBase");
        AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_center_02-Village_human_basefloor_01.tres", "centerBase");
        
        AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_outer_01-Village_human_houses_01.tres", "outer");
        AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_outer_02-Village_human_houses_01.tres", "outer");
        AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_outer_03-Village_human_houses_01.tres", "outer");
        AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_outer_01-Village_human_basefloor_01.tres", "outerBase");
        AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_outer_02-Village_human_basefloor_01.tres", "outerBase");
        AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_Village_human_outer_03-Village_human_basefloor_01.tres", "outerBase");
        
        AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_Hall base_v02.tres");
        AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_House 10.tres");
        AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_Village_human_hall_outer_01.tres");
        
        AddWaterGraphic("Ww", "assets/graphics/models/water.tres");
    }
}