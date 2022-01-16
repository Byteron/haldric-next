using System.Collections.Generic;
using Haldric.Wdk;

public class TerrainScript : TerrainLoader
{
	public override void Load()
	{
		NewBase("Gg", new List<TerrainType>() { TerrainType.Flat });        // grass
		NewBase("Rd", new List<TerrainType>() { TerrainType.Flat });        // dirt
		NewBase("Sd", new List<TerrainType>() { TerrainType.Rough });       // sand desert
		NewBase("Sm", new List<TerrainType>() { TerrainType.Rough }, -0.4f);        // sand mud
		NewBase("Sb", new List<TerrainType>() { TerrainType.Rough }, -0.4f);        // sand beach
		NewBase("Ms", new List<TerrainType>() { TerrainType.Rocky });       // mountains simple
		NewBase("Gll", new List<TerrainType>() { TerrainType.Flat });       // grass leaf litter
		NewBase("Us", new List<TerrainType>() { TerrainType.Flat });        // underground cave 

		NewBase("Ws", new List<TerrainType>() { TerrainType.ShallowWaters }, -2.5f);       // shallow water
		NewBase("Wo", new List<TerrainType>() { TerrainType.DeepWaters }, -15);     // deep water
		NewBase("Wu", new List<TerrainType>() { TerrainType.DeepWaters }, -5);     // underground water

		NewBase("Xx", new List<TerrainType>() { TerrainType.Unwalkable }, -15);		// chasm
		NewBase("Xu", new List<TerrainType>() { TerrainType.Impassable });	// impassable void

		NewOverlay("Ff", new List<TerrainType>() { TerrainType.Forested });     // forest

		NewKeep("Kh", new List<TerrainType>() { TerrainType.Fortified });       // keep human
		NewCastle("Ch", new List<TerrainType>() { TerrainType.Fortified });     // castle human

		NewHouses("Vh", new List<TerrainType>() { TerrainType.Settled });       // village human
		NewVillage("VHh", new List<TerrainType>() { TerrainType.Settled, TerrainType.Fortified });      // village hall human

		MapBaseToOverlay("Ff", "Gll");

		//      Grass  
		AddTerrainTexture("Gg", "assets/graphics/images/grass_basecolor.png");
		AddTerrainNormalTexture("Gg", "assets/graphics/images/grass_normal.png");
		AddTerrainRoughnessTexture("Gg", "assets/graphics/images/grass_roughness.png");

		//      Grass leaf litter
		AddTerrainTexture("Gll", "assets/graphics/images/leaf_litter_basecolor.png");
		AddTerrainNormalTexture("Gll", "assets/graphics/images/leaf_litter_normal.png");
		AddTerrainRoughnessTexture("Gll", "assets/graphics/images/leaf_litter_roughness.png");

		//		Sand desert
		AddTerrainTexture("Sd", "assets/graphics/images/sand_dunes_basecolor.png");
		AddTerrainNormalTexture("Sd", "assets/graphics/images/sand_dunes_normal.png");
		AddTerrainRoughnessTexture("Sd", "assets/graphics/images/sand_dunes_roughness.png");

		//		Sand beach
		AddTerrainTexture("Sb", "assets/graphics/images/sand_beach_basecolor.png");
		AddTerrainNormalTexture("Sb", "assets/graphics/images/sand_beach_normal.png");
		AddTerrainRoughnessTexture("Sb", "assets/graphics/images/sand_beach_roughness.png");

		//		Sand mud
		AddTerrainTexture("Sm", "assets/graphics/images/mud_basecolor.png");
		AddTerrainNormalTexture("Sm", "assets/graphics/images/mud_normal.png");
		AddTerrainRoughnessTexture("Sm", "assets/graphics/images/mud_roughness.png");

		//		Underground
		AddTerrainTexture("Us", "assets/graphics/images/cave_ground_basecolor.jpg");
		AddTerrainNormalTexture("Us", "assets/graphics/images/cave_ground_normal.jpg");
		AddTerrainRoughnessTexture("Us", "assets/graphics/images/cave_ground_roughness.jpg");

		//		Mountains simple
		AddTerrainTexture("Ms", "assets/graphics/images/stone_basecolor.png");
		AddTerrainNormalTexture("Ms", "assets/graphics/images/stone_normal.png");
		AddTerrainRoughnessTexture("Ms", "assets/graphics/images/stone_roughness.png");

		//		Road dirt
		AddTerrainTexture("Rd", "assets/graphics/images/dirt_basecolor.png");
		AddTerrainNormalTexture("Rd", "assets/graphics/images/dirt_normal.png");
		AddTerrainRoughnessTexture("Rd", "assets/graphics/images/dirt_roughness.png");

		//		Castle human
		AddTerrainTexture("Ch", "assets/graphics/images/dirt_castle_basecolor.png");
		AddTerrainNormalTexture("Ch", "assets/graphics/images/dirt_castle_normal.png");
		AddTerrainRoughnessTexture("Ch", "assets/graphics/images/dirt_castle_roughness.png");

		//		Keep human
		AddTerrainTexture("Kh", "assets/graphics/images/dirt_castle_basecolor.png");
		AddTerrainNormalTexture("Kh", "assets/graphics/images/dirt_castle_normal.png");
		AddTerrainRoughnessTexture("Kh", "assets/graphics/images/dirt_castle_roughness.png");

		//		Water shallow
		AddTerrainTexture("Ws", "assets/graphics/images/mud_basecolor.png");
		AddTerrainNormalTexture("Ws", "assets/graphics/images/mud_normal.png");
		AddTerrainRoughnessTexture("Ws", "assets/graphics/images/mud_roughness.png");

		//		Water deep
		AddTerrainTexture("Wo", "assets/graphics/images/mud_basecolor.png");
		AddTerrainNormalTexture("Wo", "assets/graphics/images/mud_normal.png");
		AddTerrainRoughnessTexture("Wo", "assets/graphics/images/mud_roughness.png");

		//		Water underground
		AddTerrainTexture("Wu", "assets/graphics/images/mud_basecolor.png");
		AddTerrainNormalTexture("Wu", "assets/graphics/images/mud_normal.png");
		AddTerrainRoughnessTexture("Wu", "assets/graphics/images/mud_roughness.png");

		//		Chasm - uses the same mud texture as water for a placeholder. Should be replaced eventually
		AddTerrainTexture("Xx", "assets/graphics/images/mud_basecolor.png");
		AddTerrainNormalTexture("Xx", "assets/graphics/images/mud_normal.png");
		AddTerrainRoughnessTexture("Xx", "assets/graphics/images/mud_roughness.png");
		
		//		Void - uses a placeholder black texture until some shader magic can make it pitch black
		AddTerrainTexture("Xu", "assets/graphics/images/black_1024.jpg");
		AddTerrainNormalTexture("Xu", "assets/graphics/images/normal_1024.jpg");
		AddTerrainRoughnessTexture("Xu", "assets/graphics/images/white_1024.jpg");

		// AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 1.5f, 0f));
		AddWallSegmentGraphic("Kh", "assets/graphics/models/keep_wall.tres");
		AddWallTowerGraphic("Kh", "assets/graphics/models/keep_tower.tres");

		// AddKeepPlateauGraphic("Ch", "assets/graphics/models/castle_plateau.tres", new Godot.Vector3(0f, 0.05f, 0f));
		AddWallSegmentGraphic("Ch", "assets/graphics/models/castle_wall.tres");
		AddWallTowerGraphic("Ch", "assets/graphics/models/castle_tower.tres");

		// Cliff models for terrain with unwalkable elevation difference
		AddOuterCliffGraphic("Gg", "assets/graphics/models/Cliff_grass_granit_01.tres", "assets/graphics/materials/cliff_granit_01_grass.res");
		AddOuterCliffGraphic("Gll", "assets/graphics/models/Cliff_dirt_granit_01.tres", "assets/graphics/materials/cliff_granit_01_dirt.res");
		AddOuterCliffGraphic("Ms", "assets/graphics/models/Cliff_granit_01.tres", "assets/graphics/materials/cliff_granit_01.res");
		AddOuterCliffGraphic("Rd", "assets/graphics/models/Cliff_dirt_granit_01.tres", "assets/graphics/materials/cliff_granit_01_dirt.res");
		AddOuterCliffGraphic("Sd", "assets/graphics/models/Cliff_sand_granit_01.tres", "assets/graphics/materials/cliff_granit_01_sand.res");
		AddOuterCliffGraphic("Sm", "assets/graphics/models/Cliff_dirt_granit_01.tres", "assets/graphics/materials/cliff_granit_01_dirt.res");
		AddOuterCliffGraphic("Sb", "assets/graphics/models/Cliff_sand_granit_01.tres", "assets/graphics/materials/cliff_granit_01_sand.res");
		AddOuterCliffGraphic("Ch", "assets/graphics/models/Cliff_granit_01.tres", "assets/graphics/materials/cliff_granit_01.res");
		AddOuterCliffGraphic("Kh", "assets/graphics/models/Cliff_granit_01.tres", "assets/graphics/materials/cliff_granit_01.res");
		AddOuterCliffGraphic("Xu", "assets/graphics/models/Cliff_granit_01.tres", "assets/graphics/materials/cliff_granit_01.res");

		// Cave wall models for underground tunnels
		AddInnerCliffGraphic("Us", "assets/graphics/models/Cave_Wall_1.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");
		AddInnerCliffGraphic("Us", "assets/graphics/models/Cave_Wall_2.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");
		AddInnerCliffGraphic("Us", "assets/graphics/models/Cave_Wall_3.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");
		AddInnerCliffGraphic("Us", "assets/graphics/models/Cave_Wall_4.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");

		// Cave wall models for underground water
		AddInnerCliffGraphic("Wu", "assets/graphics/models/Cave_Wall_1.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");
		AddInnerCliffGraphic("Wu", "assets/graphics/models/Cave_Wall_2.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");
		AddInnerCliffGraphic("Wu", "assets/graphics/models/Cave_Wall_3.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");
		AddInnerCliffGraphic("Wu", "assets/graphics/models/Cave_Wall_4.tres", "assets/graphics/materials/CaveWall_01.res", "Natural");

		// Forest props
		AddDecorationGraphic("Ff", "assets/graphics/models/forest_pine_center_01.tres", "center");
		AddDecorationGraphic("Ff", "assets/graphics/models/forest_pine_center_02.tres", "center");

		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/forest_pine_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/forest_pine_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/forest_pine_outer_03.tres", "outer");

		// Village props
		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_01.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_02.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_03.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/Village_human_center_04.tres", "center");

		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_04.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_05.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_outer_06.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_hall_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_hall_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/Village_human_hall_outer_05.tres", "outer");

		AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_base_01.tres");
		AddDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_center_01.tres");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_04.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_05.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/Village_human_hall_outer_06.tres", "outer");

		// Water and other terrain that uses a similar plane+shader setup
		AddWaterGraphic("Ws", "assets/graphics/models/water.tres");
		AddWaterGraphic("Wo", "assets/graphics/models/water.tres");
		AddWaterGraphic("Wu", "assets/graphics/models/water.tres");
		AddWaterGraphic("Xx", "assets/graphics/models/chasm.tres");
	}
}
