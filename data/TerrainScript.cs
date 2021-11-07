using System.Collections.Generic;
using Haldric.Wdk;

public class TerrainScript : TerrainLoader
{
	public override void Load()
	{
		NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });
		NewBase("Rd", new List<TerrainType>() { TerrainType.Flat });
		NewBase("Dd", new List<TerrainType>() { TerrainType.Rough });
		
		NewShallowWater("Ws", new List<TerrainType>() { TerrainType.ShallowWaters });
		NewDeepWater("Wo", new List<TerrainType>() { TerrainType.DeepWaters });
		
		NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });

		NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });
		NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });

		NewHouses("Vh", new List<TerrainType>() { TerrainType.Settled });
		NewVillage("VHh", new List<TerrainType>() { TerrainType.Settled });

		AddTerrainTexture("Gg", "assets/graphics/images/grass.png");
		AddTerrainNormalTexture("Gg", "assets/graphics/images/grass.png");
		AddTerrainRoughnessTexture("Gg", "assets/graphics/images/grass.png");

		AddTerrainTexture("Dd", "assets/graphics/images/mud.png");
		AddTerrainTexture("Rd", "assets/graphics/images/stone.png");
		AddTerrainTexture("Ch", "assets/graphics/images/stone.png");
		AddTerrainTexture("Kh", "assets/graphics/images/stone.png");
		AddTerrainTexture("Ws", "assets/graphics/images/mud.png");
		AddTerrainTexture("Wo", "assets/graphics/images/mud.png");

		AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 1.5f, 0f));
		AddWallSegmentGraphic("Kh", "assets/graphics/models/keep_wall.tres");
		AddWallTowerGraphic("Kh", "assets/graphics/models/keep_tower.tres");

		// AddKeepPlateauGraphic("Ch", "assets/graphics/models/castle_plateau.tres", new Godot.Vector3(0f, 0.05f, 0f));
		AddWallSegmentGraphic("Ch", "assets/graphics/models/castle_wall.tres");
		AddWallTowerGraphic("Ch", "assets/graphics/models/castle_tower.tres");

		AddDecorationGraphic("Ff", "assets/graphics/models/forest_pine_center_01.tres", "center");
		AddDecorationGraphic("Ff", "assets/graphics/models/forest_pine_center_02.tres", "center");

		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/forest_pine_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/forest_pine_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/forest_pine_outer_03.tres", "outer");

		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_01.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_02.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_03.tres", "center");
		
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_04.tres", "outer");
		
		AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_base_01.tres");
		AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_center_01.tres");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_03.tres", "outer");
		
		AddWaterGraphic("Ws", "assets/graphics/models/water.tres");
		AddWaterGraphic("Wo", "assets/graphics/models/water.tres");
	}
}
