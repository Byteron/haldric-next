using System.Collections.Generic;
using RelEcs;

public class LoadTerrainDataTrigger
{
}

public class LoadTerrainDataSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        foreach (var _ in this.Receive<LoadTerrainDataTrigger>())
        {
            var terrainData = new TerrainData();
            this.AddOrReplaceElement(terrainData);

            terrainData.Terrains = Loader.LoadJson<Dictionary<string, TerrainInfo>>("res://data/terrain.json");

            foreach (var (code, data) in terrainData.Terrains)
            {
                var entityBuilder = this.Spawn();

                if (data.IsBase) entityBuilder.Add<IsBaseTerrain>();
                else entityBuilder.Add<IsOverlayTerrain>();

                entityBuilder.Add(new TerrainCode { Value = data.Code })
                    .Add(new TerrainTypes { List = data.Types })
                    .Add(new ElevationOffset { Value = data.ElevationOffset });

                if (data.CanRecruitFrom) entityBuilder.Add<CanRecruitFrom>();
                if (data.CanRecruitTo) entityBuilder.Add<CanRecruitTo>();
                if (data.GivesIncome) entityBuilder.Add<GivesIncome>();
                if (data.IsCapturable) entityBuilder.Add<IsCapturable>();
                if (data.Heals) entityBuilder.Add<Heals>();

                terrainData.TerrainEntities.Add(code, entityBuilder.Id());
            }
        }
    }
}