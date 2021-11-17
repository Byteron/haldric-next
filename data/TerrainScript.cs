using System.Collections.Generic;
using Haldric.Wdk;

public class TerrainScript : TerrainLoader
{
	public override void Load()
	{
		NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });		// grass
		NewBase("Rd", new List<TerrainType>() { TerrainType.Flat });		// dirt
		NewBase("Sd", new List<TerrainType>() { TerrainType.Rough });		// sand desert
		NewBase("Sm", new List<TerrainType>() { TerrainType.Rough }, -0.4f);		// sand mud
		NewBase("Sb", new List<TerrainType>() { TerrainType.Rough }, -0.4f);		// sand beach
		NewBase("Ms", new List<TerrainType>() { TerrainType.Rocky });		// mountains simple

		NewShallowWater("Ws", new List<TerrainType>() { TerrainType.ShallowWaters });
		NewDeepWater("Wo", new List<TerrainType>() { TerrainType.DeepWaters });

		NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });

		NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });
		NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });

		NewHouses("Vh", new List<TerrainType>() { TerrainType.Settled });
		NewVillage("VHh", new List<TerrainType>() { TerrainType.Settled, TerrainType.Fortified });

//      Grass  
		AddTerrainTexture("Gg", "assets/graphics/images/grass_basecolor.jpg");
		AddTerrainNormalTexture("Gg", "assets/graphics/images/grass_normal.jpg");
		AddTerrainRoughnessTexture("Gg", "assets/graphics/images/grass_roughness.jpg");

//		Sand desert
		AddTerrainTexture("Sd", "assets/graphics/images/sand_dunes_basecolor.jpg");
		AddTerrainNormalTexture("Sd", "assets/graphics/images/sand_dunes_normal.jpg");
		AddTerrainRoughnessTexture("Sd", "assets/graphics/images/sand_dunes_roughness.jpg");

//		Sand beach
		AddTerrainTexture("Sb", "assets/graphics/images/sand_beach_basecolor.jpg");
		AddTerrainNormalTexture("Sb", "assets/graphics/images/sand_beach_normal.jpg");
		AddTerrainRoughnessTexture("Sb", "assets/graphics/images/sand_beach_roughness.jpg");

//		Sand mud
		AddTerrainTexture("Sm", "assets/graphics/images/mud_basecolor.jpg");
		AddTerrainNormalTexture("Sm", "assets/graphics/images/mud_normal.jpg");
		AddTerrainRoughnessTexture("Sm", "assets/graphics/images/mud_roughness.jpg");

//		Mountains rocky
		AddTerrainTexture("Ms", "assets/graphics/images/stone_basecolor.jpg");
		AddTerrainNormalTexture("Ms", "assets/graphics/images/stone_normal.jpg");
		AddTerrainRoughnessTexture("Ms", "assets/graphics/images/stone_roughness.jpg");
		
//		Road dirt
		AddTerrainTexture("Rd", "assets/graphics/images/dirt_basecolor.jpg");
		AddTerrainNormalTexture("Rd", "assets/graphics/images/dirt_normal.jpg");
		AddTerrainRoughnessTexture("Rd", "assets/graphics/images/dirt_roughness.jpg");

//		Castle human
		AddTerrainTexture("Ch", "assets/graphics/images/dirt_castle_basecolor.jpg");
		AddTerrainNormalTexture("Ch", "assets/graphics/images/dirt_castle_normal.jpg");
		AddTerrainRoughnessTexture("Ch", "assets/graphics/images/dirt_castle_roughness.jpg");

//		Keep human
		AddTerrainTexture("Kh", "assets/graphics/images/dirt_castle_basecolor.jpg");
		AddTerrainNormalTexture("Kh", "assets/graphics/images/dirt_castle_normal.jpg");
		AddTerrainRoughnessTexture("Kh", "assets/graphics/images/dirt_castle_roughness.jpg");

//		Water shallow
		AddTerrainTexture("Ws", "assets/graphics/images/mud_basecolor.jpg");
		AddTerrainNormalTexture("Ws", "assets/graphics/images/mud_normal.jpg");
		AddTerrainRoughnessTexture("Ws", "assets/graphics/images/mud_roughness.jpg");
		
//		Water deep
		AddTerrainTexture("Wo", "assets/graphics/images/mud_basecolor.jpg");
		AddTerrainNormalTexture("Wo", "assets/graphics/images/mud_normal.jpg");
		AddTerrainRoughnessTexture("Wo", "assets/graphics/images/mud_roughness.jpg");
		
		// AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 1.5f, 0f));
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
