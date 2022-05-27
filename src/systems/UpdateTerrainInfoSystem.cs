using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class UpdateTerrainInfoSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation)) return;

        if (!commands.TryGetElement<TerrainPanel>(out var terrainPanel)) return;

        var locEntity = hoveredLocation.Entity;
        
        if (locEntity is null || !locEntity.IsAlive) return;

        var mobility = new Mobility
        {
            Dict = new Dictionary<TerrainType, int>()
        };

        if (commands.TryGetElement<SelectedLocation>(out var selectedLocation))
        {
            if (selectedLocation.Entity.Has<HasUnit>())
            {
                var unitEntity = selectedLocation.Entity.Get<HasUnit>().Entity;
                mobility = unitEntity.Get<Mobility>();
            }
        }

        var coords = locEntity.Get<Coords>();

        var elevation = locEntity.Get<Elevation>();
        var baseTerrain = locEntity.Get<HasBaseTerrain>();
        var baseTerrainCode = baseTerrain.Entity.Get<TerrainCode>();
        var overlayTerrainCode = "";

        var terrainTypes = TerrainTypes.FromLocEntity(locEntity);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            var overlayTerrain = locEntity.Get<HasOverlayTerrain>();
            var overlayCode = overlayTerrain.Entity.Get<TerrainCode>();
            overlayTerrainCode = "^" + overlayCode.Value;
        }

        var text = $"Coords: {coords.Offset().x}, {coords.Offset().z}";
        text += $"\nElevation: {elevation.Value}";
        text += $"\nTerrain: {baseTerrainCode.Value}{overlayTerrainCode}";
        text += $"\nTypes: {terrainTypes}";
        text += $"\nDefense: {(int)(100 * terrainTypes.GetDefense())}%";
        text += $"\nCost: {terrainTypes.GetMovementCost(mobility)}";

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

        terrainPanel.UpdateInfo(text);
    }
}