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
        if (!world.TryGetResource<Map>(out var map))
        {
            return;
        }

        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var editorView = world.GetResource<EditorView>();
        
        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive())
        {
            return;
        }
        
        var locations = map.Locations;

        ref var hoveredCoords = ref locEntity.Get<Coords>();
        if (hoveredCoords.Cube != _previousCoords && Input.IsActionPressed("editor_select"))
        {
            _previousCoords = hoveredCoords.Cube;

            var chunks = new List<Vector3i>();

            foreach (var cube in Hex.GetCellsInRange(hoveredCoords.Cube, editorView.BrushSize))
            {
                if (!locations.Has(cube))
                {
                    return;
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
                return;
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

    private void EditLocation(EditorView editorView, EcsEntity locEntity)
    {
        ref HasBaseTerrain baseTerrain = ref locEntity.Get<HasBaseTerrain>();
        ref var elevation = ref locEntity.Get<Elevation>();

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

                baseTerrain.Entity = editorView.TerrainEntity;

                if (baseTerrain.Entity.Has<HasShallowWater>())
                {
                    elevation.Offset = Metrics.ShallowWaterOffset;
                }
                else if (baseTerrain.Entity.Has<HasDeepWater>())
                {
                    elevation.Offset = Metrics.DeepWaterOffset;
                }
            }
        }

        if (editorView.UseElevation)
        {
            elevation.Value = editorView.Elevation;
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