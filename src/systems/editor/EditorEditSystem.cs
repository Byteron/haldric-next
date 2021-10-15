using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public class EditorEditSystem : IEcsSystem
{
    Node3D _parent;

    Vector3 _previousCoords;

    public EditorEditSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var hoveredLocationQuery = world.Query<HoveredLocation>().End();

        if (!world.HasResource<Map>())
        {
            return;
        }

        var map = world.GetResource<Map>();

        var editorView = world.GetResource<EditorView>();

        foreach (var hoveredLocationEntity in hoveredLocationQuery)
        {
            ref var locations = ref map.Locations;
            var locEntity = hoveredLocationQuery.Get<HoveredLocation>(hoveredLocationEntity).Entity;

            if (!locEntity.IsAlive())
            {
                return;
            }

            ref var hoveredCoords = ref locEntity.Get<Coords>();
            if (hoveredCoords.Cube != _previousCoords && Input.IsActionPressed("editor_select"))
            {
                _previousCoords = hoveredCoords.Cube;

                var chunks = new List<Vector3i>();

                foreach (var cube in Hex.GetCellsInRange(hoveredCoords.Cube, editorView.BrushSize))
                {
                    if (!locations.Has(cube))
                    {
                        continue;
                    }

                    var nLocEntity = locations.Get(cube);
                    EditLocation(editorView, nLocEntity);

                    var chunkCell = nLocEntity.Get<Vector3i>();

                    if (!chunks.Contains(chunkCell))
                    {
                        chunks.Add(chunkCell);
                        chunks.Add(chunkCell + new Vector3i(1, 0, 1));
                        chunks.Add(chunkCell + new Vector3i(1, 0, 0));
                        chunks.Add(chunkCell + new Vector3i(1, 0, -1));
                        chunks.Add(chunkCell + new Vector3i(-1, 0, 1));
                        chunks.Add(chunkCell + new Vector3i(-1, 0, 0));
                        chunks.Add(chunkCell + new Vector3i(-1, 0, -1));
                        chunks.Add(chunkCell + new Vector3i(0, 0, 1));
                        chunks.Add(chunkCell + new Vector3i(0, 0, -1));
                    }
                }

                if (!editorView.UseTerrain && ! editorView.UseElevation)
                {
                    continue;
                }
                
                if (editorView.TerrainEntity.Has<HasOverlayTerrain>())
                {
                    SendFeaturesUpdateEvent(world, chunks);
                }
                else
                {
                    SendUpdateMapEvent(world, chunks);
                }
            }
        }
    }

    private void EditLocation(EditorView editorView, EcsEntity locEntity)
    {
        ref var baseTerrainEntity = ref locEntity.Get<HasBaseTerrain>().Entity;

        ref var elevation = ref locEntity.Get<Elevation>();

        var wasWater = baseTerrainEntity.Has<HasWater>() ? true : false;

        if (editorView.UseTerrain)
        {
            if (editorView.TerrainEntity.Has<IsOverlayTerrain>())
            {
                if (!locEntity.Has<HasOverlayTerrain>())
                {
                    locEntity.Add<HasOverlayTerrain>();
                }

                locEntity.Get<HasOverlayTerrain>().Entity = editorView.TerrainEntity;
            }
            else
            {
                if (locEntity.Has<HasOverlayTerrain>())
                {
                    locEntity.Remove<HasOverlayTerrain>();
                }

                baseTerrainEntity = editorView.TerrainEntity;
            }

            if (!wasWater && baseTerrainEntity.Has<HasWater>())
            {
                elevation.Level -= 1;
            }
        }

        if (editorView.UseElevation)
        {
            elevation.Level = editorView.Elevation;

            if (baseTerrainEntity.Has<HasWater>())
            {
                elevation.Level -= 1;
            }
        }
    }

    private void SendUpdateMapEvent(EcsWorld world, List<Vector3i> chunks)
    {
        world.Spawn().Add(new UpdateMapEvent(chunks));
    }

    private void SendFeaturesUpdateEvent(EcsWorld world, List<Vector3i> chunks)
    {
        world.Spawn().Add(new UpdateTerrainFeaturePopulatorEvent(chunks));
    }
}