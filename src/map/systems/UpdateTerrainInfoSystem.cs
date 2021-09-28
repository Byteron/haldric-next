using Godot;
using Bitron.Ecs;

public class UpdateTerrainInfoSystem : IEcsSystem
{
    EcsFilter<MapCursor> _filter;

    public void Run(EcsWorld world)
    {
        foreach (var i in _filter)
        {
            var cursorEntity = _filter.GetEntity(i);
            var locEntity = cursorEntity.Get<HoveredLocation>().Entity;

            if (locEntity == EcsEntity.Null)
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

            Main.Instance.GetTree().CallGroup("TerrainLabel", "set", "text", string.Format("Terrain: {0}{1}\nElevation: {2}", baseTerrainCode, overlayTerrainCode, elevation));
        }
    }
}