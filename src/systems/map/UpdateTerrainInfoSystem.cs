using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class UpdateTerrainInfoSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        if (!commands.TryGetElement<TerrainPanel>(out var terrainPanel))
        {
            return;
        }

        var locEntity = hoveredLocation.Entity;

        
        if (!locEntity.IsAlive)
        {
            return;
        }

        Mobility mobility = new Mobility();
        mobility.Dict = new Dictionary<TerrainType, int>();

        if (commands.TryGetElement<SelectedLocation>(out var selectedLocation))
        {
            if (selectedLocation.Entity.Has<HasUnit>())
            {
                var unitEntity = selectedLocation.Entity.Get<HasUnit>().Entity;
                mobility = unitEntity.Get<Mobility>();
            }
        }

        ref Coords coords = ref locEntity.Get<Coords>();

        ref var elevation = ref locEntity.Get<Elevation>();
        ref var baseTerrain = ref locEntity.Get<HasBaseTerrain>();
        ref var baseTerrainCode = ref baseTerrain.Entity.Get<TerrainCode>();
        var overlayTerrainCode = "";

        var terrainTypes = TerrainTypes.FromLocEntity(locEntity);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            ref var overlayTerrain = ref locEntity.Get<HasOverlayTerrain>();
            ref var overlayCode = ref overlayTerrain.Entity.Get<TerrainCode>();
            overlayTerrainCode = "^" + overlayCode.Value;
        }

        string text = $"Coords: {coords.Offset().x}, {coords.Offset().z}";
        text += $"\nElevation: {elevation.Value}";
        text += $"\nTerrain: {baseTerrainCode.Value}{overlayTerrainCode}";
        text += $"\nTypes: {terrainTypes}";
        text += $"\nDefense: {(int)(100 * terrainTypes.GetDefense())}%";
        text += $"\nCost: {terrainTypes.GetMovementCost(mobility)}";

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

        terrainPanel.UpdateInfo(text);
    }
}