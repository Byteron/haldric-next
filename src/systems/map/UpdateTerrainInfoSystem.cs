using Godot;
using Bitron.Ecs;

public class UpdateTerrainInfoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.HasResource<HUDView>())
        {
            return;
        }
        
        var hudView = world.GetResource<HUDView>();

        var query = world.Query<HoveredLocation>().End();

        foreach (var entityId in query)
        {
            string text = "";

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

            text = string.Format("Terrain: {0}{1}\nElevation: {2}", baseTerrainCode, overlayTerrainCode, elevation);
            
            if (locEntity.Has<Castle>())
            {
                ref var castle = ref locEntity.Get<Castle>();

                text += "\nCastle: " + castle.List.Count;
            }

            if (locEntity.Has<Village>())
            {
                ref var village = ref locEntity.Get<Village>();

                text += "\nVillage: " + village.List.Count;
            }

            hudView.TerrainLabel.Text = text;
        }
    }
}