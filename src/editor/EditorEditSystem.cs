using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

public struct Editor { }

public class EditorEditSystem : IEcsInitSystem, IEcsRunSystem
{
    EcsWorld _world;

    Node3D _parent;

    EcsFilter<HoveredLocation> _hoveredLocations;
    EcsFilter<Locations, Map> _locations;

    EcsFilter<Editor> _editors;

    Vector3 _previousCoords;

    public EditorEditSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var editorEntity = _world.NewEntity();

        var editorView = Scenes.Instance.EditorView.Instantiate<EditorView>();
        _parent.AddChild(editorView);

        editorEntity.Replace(new NodeHandle<EditorView>(editorView));
        editorEntity.Get<Editor>();

        _world.NewEntity().Replace(new CreateMapEvent(40, 40));
    }

    public void Run()
    {
        foreach (var i in _locations)
        {
            if (_hoveredLocations.IsEmpty() || _locations.IsEmpty() || _editors.IsEmpty())
            {
                return;
            }

            ref var locations = ref _locations.GetEntity(0).Get<Locations>();
            var locEntity = _hoveredLocations.GetEntity(0).Get<HoveredLocation>().Entity;

            if (locEntity == EcsEntity.Null)
            {
                return;
            }

            var editorEntity = _editors.GetEntity(0);

            ref var editorView = ref editorEntity.Get<NodeHandle<EditorView>>().Node;

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
                    EditLocation(editorEntity, nLocEntity);

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
                    SendFeaturesUpdateEvent(chunks);
                }
                else
                {
                    SendUpdateMapEvent(chunks);
                }
            }
        }
    }

    private void EditLocation(EcsEntity editorEntity, EcsEntity locEntity)
    {
        ref var editorView = ref editorEntity.Get<NodeHandle<EditorView>>().Node;

        ref var baseTerrainEntity = ref locEntity.Get<HasBaseTerrain>().Entity;

        ref var elevation = ref locEntity.Get<Elevation>();

        var wasWater = baseTerrainEntity.Has<HasWater>() ? true : false;

        if (editorView.UseTerrain)
        {
            if (editorView.TerrainEntity.Has<OverlayTerrain>())
            {
                locEntity.Get<HasOverlayTerrain>().Entity = editorView.TerrainEntity;
            }
            else
            {
                if (locEntity.Has<HasOverlayTerrain>())
                {
                    locEntity.Del<HasOverlayTerrain>();
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

    private void SendUpdateMapEvent(List<Vector3i> chunks)
    {
        var eventEntity = _world.NewEntity();
        eventEntity.Replace(new UpdateMapEvent(chunks));
    }

    private void SendFeaturesUpdateEvent(List<Vector3i> chunks)
    {
        var eventEntity = _world.NewEntity();
        eventEntity.Replace(new UpdateTerrainFeaturePopulatorEvent(chunks));
    }
}