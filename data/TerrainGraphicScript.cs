using System.Collections.Generic;

namespace Haldric;

public class TerrainGraphicInfo
{
	public string Code;
	public string Texture;
	public string Normal;
	public string Roughness;
	public List<TerrainModelInfo> Decorations;
	public List<TerrainModelInfo> DirectionalDecorations;
	public List<TerrainModelInfo> InnerCliffs;
	public List<TerrainModelInfo> OuterCliffs;
}

public class TerrainModelInfo
{
	string Group;
	string Model;
	string Material;
}

public class TerrainGraphicScript : TerrainGraphicLoader
{
	public override void Load()
	{
		//      Grass  
		AddTerrainTexture("Gg", "assets/graphics/images/terrain/grass_basecolor.jpg");
		AddTerrainNormalTexture("Gg", "assets/graphics/images/terrain/grass_normal.jpg");
		AddTerrainRoughnessTexture("Gg", "assets/graphics/images/terrain/grass_roughness.jpg");

		//      Grass leaf litter
		AddTerrainTexture("Gll", "assets/graphics/images/terrain/leaf_litter_basecolor.png");
		AddTerrainNormalTexture("Gll", "assets/graphics/images/terrain/leaf_litter_normal.png");
		AddTerrainRoughnessTexture("Gll", "assets/graphics/images/terrain/leaf_litter_roughness.png");

		//		Sand desert
		AddTerrainTexture("Sd", "assets/graphics/images/terrain/sand_dunes_basecolor.png");
		AddTerrainNormalTexture("Sd", "assets/graphics/images/terrain/sand_dunes_normal.png");
		AddTerrainRoughnessTexture("Sd", "assets/graphics/images/terrain/sand_dunes_roughness.png");

		//		Sand beach
		AddTerrainTexture("Sb", "assets/graphics/images/terrain/sand_beach_basecolor.png");
		AddTerrainNormalTexture("Sb", "assets/graphics/images/terrain/sand_beach_normal.png");
		AddTerrainRoughnessTexture("Sb", "assets/graphics/images/terrain/sand_beach_roughness.png");

		//		Sand mud
		AddTerrainTexture("Sm", "assets/graphics/images/terrain/mud_basecolor.png");
		AddTerrainNormalTexture("Sm", "assets/graphics/images/terrain/mud_normal.png");
		AddTerrainRoughnessTexture("Sm", "assets/graphics/images/terrain/mud_roughness.png");

		//		Underground
		AddTerrainTexture("Us", "assets/graphics/images/terrain/cave_ground_basecolor.jpg");
		AddTerrainNormalTexture("Us", "assets/graphics/images/terrain/cave_ground_normal.jpg");
		AddTerrainRoughnessTexture("Us", "assets/graphics/images/terrain/cave_ground_roughness.jpg");

		//		Underground impassable
		AddTerrainTexture("Uu", "assets/graphics/images/terrain/cave_ground_basecolor.jpg");
		AddTerrainNormalTexture("Uu", "assets/graphics/images/terrain/cave_ground_normal.jpg");
		AddTerrainRoughnessTexture("Uu", "assets/graphics/images/terrain/cave_ground_roughness.jpg");

		//		Mountains simple
		AddTerrainTexture("Ms", "assets/graphics/images/terrain/stone_basecolor.png");
		AddTerrainNormalTexture("Ms", "assets/graphics/images/terrain/stone_normal.png");
		AddTerrainRoughnessTexture("Ms", "assets/graphics/images/terrain/stone_roughness.png");

		//		Road dirt
		AddTerrainTexture("Rd", "assets/graphics/images/terrain/dirt_basecolor.png");
		AddTerrainNormalTexture("Rd", "assets/graphics/images/terrain/dirt_normal.png");
		AddTerrainRoughnessTexture("Rd", "assets/graphics/images/terrain/dirt_roughness.png");

		//		Castle human
		AddTerrainTexture("Ch", "assets/graphics/images/terrain/dirt_castle_basecolor.png");
		AddTerrainNormalTexture("Ch", "assets/graphics/images/terrain/dirt_castle_normal.png");
		AddTerrainRoughnessTexture("Ch", "assets/graphics/images/terrain/dirt_castle_roughness.png");

		//		Keep human
		AddTerrainTexture("Kh", "assets/graphics/images/terrain/dirt_castle_basecolor.png");
		AddTerrainNormalTexture("Kh", "assets/graphics/images/terrain/dirt_castle_normal.png");
		AddTerrainRoughnessTexture("Kh", "assets/graphics/images/terrain/dirt_castle_roughness.png");

		//		Water shallow
		AddTerrainTexture("Ws", "assets/graphics/images/terrain/mud_basecolor.png");
		AddTerrainNormalTexture("Ws", "assets/graphics/images/terrain/mud_normal.png");
		AddTerrainRoughnessTexture("Ws", "assets/graphics/images/terrain/mud_roughness.png");

		//		Water deep
		AddTerrainTexture("Wo", "assets/graphics/images/terrain/mud_basecolor.png");
		AddTerrainNormalTexture("Wo", "assets/graphics/images/terrain/mud_normal.png");
		AddTerrainRoughnessTexture("Wo", "assets/graphics/images/terrain/mud_roughness.png");

		//		Water underground
		AddTerrainTexture("Wu", "assets/graphics/images/terrain/mud_basecolor.png");
		AddTerrainNormalTexture("Wu", "assets/graphics/images/terrain/mud_normal.png");
		AddTerrainRoughnessTexture("Wu", "assets/graphics/images/terrain/mud_roughness.png");

		//		Chasm - uses the same mud texture as water for a placeholder. Should be replaced eventually
		AddTerrainTexture("Xx", "assets/graphics/images/terrain/mud_basecolor.png");
		AddTerrainNormalTexture("Xx", "assets/graphics/images/terrain/mud_normal.png");
		AddTerrainRoughnessTexture("Xx", "assets/graphics/images/terrain/mud_roughness.png");
		
		//		Void - uses a placeholder black texture until some shader magic can make it pitch black
		AddTerrainTexture("Xu", "assets/graphics/images/terrain/black_1024.jpg");
		AddTerrainNormalTexture("Xu", "assets/graphics/images/terrain/normal_1024.jpg");
		AddTerrainRoughnessTexture("Xu", "assets/graphics/images/terrain/white_1024.jpg");

		// // AddKeepPlateauGraphic("Kh", "assets/graphics/models/keep_plateau.tres", new Godot.Vector3(0f, 1.5f, 0f)); // WARNING : PLATEAU MESH REMOVED
		AddWallSegmentGraphic("Kh", "assets/graphics/models/props/keep_human_wall_01.tres");
		AddWallTowerGraphic("Kh", "assets/graphics/models/props/keep_human_tower_01.tres");
		//
		// // AddKeepPlateauGraphic("Ch", "assets/graphics/models/castle_plateau.tres", new Godot.Vector3(0f, 0.05f, 0f)); // WARNING : PLATEAU MESH REMOVED
		AddWallSegmentGraphic("Ch", "assets/graphics/models/props/castle_human_wall_01.tres");
		AddWallTowerGraphic("Ch", "assets/graphics/models/props/castle_human_tower_01.tres");
		//
		// // Cliff models for terrain with unwalkable elevation difference
		AddOuterCliffGraphic("Gg", "assets/graphics/models/props/cliff_granit_01_grass.tres", "assets/graphics/materials/props/cliff_granit_01_grass.tres");
		AddOuterCliffGraphic("Gll", "assets/graphics/models/props/cliff_granit_01_dirt.tres", "assets/graphics/materials/props/cliff_granit_01_dirt.tres");
		AddOuterCliffGraphic("Ms", "assets/graphics/models/props/cliff_granit_01.tres", "assets/graphics/materials/props/cliff_granit_01.tres");
		AddOuterCliffGraphic("Rd", "assets/graphics/models/props/cliff_granit_01_dirt.tres", "assets/graphics/materials/props/cliff_granit_01_dirt.tres");
		AddOuterCliffGraphic("Sd", "assets/graphics/models/props/cliff_granit_01_sand.tres", "assets/graphics/materials/props/cliff_granit_01_sand.tres");
		AddOuterCliffGraphic("Sm", "assets/graphics/models/props/cliff_granit_01_dirt.tres", "assets/graphics/materials/props/cliff_granit_01_dirt.tres");
		AddOuterCliffGraphic("Sb", "assets/graphics/models/props/cliff_granit_01_sand.tres", "assets/graphics/materials/props/cliff_granit_01_sand.tres");
		AddOuterCliffGraphic("Ch", "assets/graphics/models/props/cliff_granit_01.tres", "assets/graphics/materials/props/cliff_granit_01.tres");
		AddOuterCliffGraphic("Kh", "assets/graphics/models/props/cliff_granit_01.tres", "assets/graphics/materials/props/cliff_granit_01.tres");
		AddOuterCliffGraphic("Xu", "assets/graphics/models/props/cliff_granit_01.tres", "assets/graphics/materials/props/cliff_granit_01.tres");
		//
		// // Cave wall models for underground tunnels
		// AddInnerCliffGraphic("Us", "assets/graphics/models/cave_wall_1.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Us", "assets/graphics/models/cave_wall_2.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Us", "assets/graphics/models/cave_wall_3.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Us", "assets/graphics/models/cave_wall_4.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		//
		// AddInnerCliffGraphic("Uu", "assets/graphics/models/cave_wall_1.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Uu", "assets/graphics/models/cave_wall_2.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Uu", "assets/graphics/models/cave_wall_3.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Uu", "assets/graphics/models/cave_wall_4.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		//
		// // Cave wall models for underground water
		// AddInnerCliffGraphic("Wu", "assets/graphics/models/cave_wall_1.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Wu", "assets/graphics/models/cave_wall_2.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Wu", "assets/graphics/models/cave_wall_3.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		// AddInnerCliffGraphic("Wu", "assets/graphics/models/cave_wall_4.tres", "assets/graphics/materials/cave_wall_01.res", "Natural");
		//
		// // Cave props
		// AddDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_L_01.tres", "center");
		// AddDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_L_02.tres", "center");
		// AddDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_L_03.tres", "center");
		// AddDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_L_04.tres", "center");
		//
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_01.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_02.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_03.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_04.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_05.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_06.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_07.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_08.tres", "outer");
		// AddDirectionalDecorationGraphic("Uu", "assets/graphics/models/cave_props_stalagmites_M_01.tres", "outer");
		//
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_01.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_02.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_03.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_04.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_05.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_06.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_07.tres", "outer");
		// AddDirectionalDecorationGraphic("Us", "assets/graphics/models/cave_props_stalagmites_08.tres", "outer");
		//
		// // Forest props
		AddDecorationGraphic("Ff", "assets/graphics/models/props/forest_pine_center_01.tres", "center");
		AddDecorationGraphic("Ff", "assets/graphics/models/props/forest_pine_center_02.tres", "center");
		
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/props/forest_pine_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/props/forest_pine_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("Ff", "assets/graphics/models/props/forest_pine_outer_03.tres", "outer");
		//
		// // Toadstool Forest props
		// AddDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_center_01.tres", "center");
		// AddDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_center_02.tres", "center");
		// AddDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_center_03.tres", "center");
		// AddDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_center_04.tres", "center");
		//
		// AddDirectionalDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_outer_01.tres", "outer");
		// AddDirectionalDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_outer_02.tres", "outer");
		// AddDirectionalDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_outer_03.tres", "outer");
		// AddDirectionalDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_outer_04.tres", "outer");
		// AddDirectionalDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_outer_05.tres", "outer");
		// AddDirectionalDecorationGraphic("Tt", "assets/graphics/models/shroom_forest_outer_06.tres", "outer");
		//
		// Village props
		AddDecorationGraphic("Vh", "assets/graphics/models/props/village_human_center_01.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/props/village_human_center_02.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/props/village_human_center_03.tres", "center");
		AddDecorationGraphic("Vh", "assets/graphics/models/props/village_human_center_04.tres", "center");
		
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_outer_04.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_outer_05.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_outer_06.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_hall_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_hall_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("Vh", "assets/graphics/models/props/village_human_hall_outer_05.tres", "outer");
		
		AddDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_base_01.tres");
		AddDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_center_01.tres");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_outer_01.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_outer_02.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_outer_03.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_outer_04.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_outer_05.tres", "outer");
		AddDirectionalDecorationGraphic("VHh", "assets/graphics/models/props/village_human_hall_outer_06.tres", "outer");
		//
		// // Water and other terrain that uses a similar plane+shader setup
		AddWaterGraphic("Ws", "assets/graphics/models/water.tres");
		AddWaterGraphic("Wo", "assets/graphics/models/water.tres");
		AddWaterGraphic("Wu", "assets/graphics/models/water.tres");
		AddWaterGraphic("Xx", "assets/graphics/models/chasm.tres");
	}
}
