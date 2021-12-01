using System.Collections.Generic;
using Bitron.Ecs;

public class MapDataPlayer
{
    public Coords Coords;
    public int Side;
}

public class MapDataLocation
{
    public Coords Coords;
    public List<string> Terrain = new List<string>();
    public int Elevation;

    public static MapDataLocation FromLocEntity(EcsEntity locEntity)
    {
        var data = new MapDataLocation();
        data.Coords = locEntity.Get<Coords>();
        data.Elevation = locEntity.Get<Elevation>().Value;

        ref var baseTerrain = ref locEntity.Get<HasBaseTerrain>();
        var entity = baseTerrain.Entity;
        ref var baseTerrainCode = ref entity.Get<TerrainCode>();

        data.Terrain.Add(baseTerrainCode.Value);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            ref var overlayTerrain = ref locEntity.Get<HasOverlayTerrain>();
            var overlayEntity = overlayTerrain.Entity;
            ref var overlayTerrainCode = ref overlayEntity.Get<TerrainCode>();

            data.Terrain.Add(overlayTerrainCode.Value);
        }

        return data;
    }
}

public class MapData
{
    public int Width = 0;
    public int Height = 0;
    public List<MapDataPlayer> Players = new List<MapDataPlayer>();
    public List<MapDataLocation> Locations = new List<MapDataLocation>();
}