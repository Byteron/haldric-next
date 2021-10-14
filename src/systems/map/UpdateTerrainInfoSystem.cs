using Godot;
using Bitron.Ecs;

public class UpdateTerrainInfoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.HasResource<HudView>())
        {
            return;
        }
        
        var hudView = world.GetResource<HudView>();

        var query = world.Query<HoveredLocation>().End();
        foreach (var entityId in query)
        {
            var locEntity = query.Get<HoveredLocation>(entityId).Entity;

            if (!locEntity.IsAlive())
            {
                continue;
            }

            var elevation = locEntity.Get<Elevation>().Level;
            var baseTerrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            var baseTerrainCode = baseTerrainEntity.Get<TerrainCode>().Value;
            var overlayTerrainCode = "";

            if (locEntity.Has<HasOverlayTerrain>())
            {
                var overlayTerrainEntity = locEntity.Get<HasOverlayTerrain>().Entity;
                overlayTerrainCode = "^" + overlayTerrainEntity.Get<TerrainCode>().Value;
            }

            hudView.TerrainLabel.Text = string.Format("Terrain: {0}{1}\nElevation: {2}", baseTerrainCode, overlayTerrainCode, elevation);
        }
    }
}