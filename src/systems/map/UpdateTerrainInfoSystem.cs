using Bitron.Ecs;

public class UpdateTerrainInfoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.HasResource<HoveredLocation>() || !world.HasResource<HUDView>())
        {
            return;
        }
        
        var hudView = world.GetResource<HUDView>();

        var hoveredLocation = world.GetResource<HoveredLocation>();

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive())
        {
            return;
        }

        var coords = locEntity.Get<Coords>();

        var elevation = locEntity.Get<Elevation>().Value;
        var baseTerrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var baseTerrainCode = baseTerrainEntity.Get<TerrainCode>().Value;
        var overlayTerrainCode = "";

        var terrainTypes = TerrainTypes.FromLocEntity(locEntity);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            var overlayTerrainEntity = locEntity.Get<HasOverlayTerrain>().Entity;
            overlayTerrainCode = "^" + overlayTerrainEntity.Get<TerrainCode>().Value;
        }

        string text = $"Coords: {coords.Offset.x}, {coords.Offset.z}";
        text += $"\nElevation: {elevation}";
        text += $"\nTerrain: {baseTerrainCode}{overlayTerrainCode}";
        text += $"\nTypes: {terrainTypes}";
        text += $"\nDefense: {(int)(100 * terrainTypes.GetDefense())}%";
        text += $"\nCost: {terrainTypes.GetMovementCost()}";

        if (locEntity.Has<Castle>())
        {
            var castle = locEntity.Get<Castle>();

            text += "\nCastle: " + castle.List.Count;
        }

        if (locEntity.Has<Village>())
        {
            var village = locEntity.Get<Village>();

            text += "\nVillage: " + village.List.Count;
        }

        hudView.TerrainLabel.Text = text;
    }
}