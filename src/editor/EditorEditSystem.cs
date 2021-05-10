using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

public struct Editor { }

public class EditorEditSystem : IEcsInitSystem, IEcsRunSystem
{
    EcsWorld _world;

    Node3D _parent;

    EcsFilter<HoveredCoords> _hoveredCoords;
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

        var editorView = Scenes.Instance.EditorView.Instance<EditorView>();
        _parent.AddChild(editorView);

        editorEntity.Replace(new NodeHandle<EditorView>(editorView));
        editorEntity.Get<Editor>();
    }

    public void Run()
    {
        foreach (var i in _locations)
        {
            if (_hoveredCoords.IsEmpty() || _locations.IsEmpty() || _editors.IsEmpty())
            {
                return;
            }

            var editorEntity = _editors.GetEntity(0);

            ref var editorView = ref editorEntity.Get<NodeHandle<EditorView>>().Node;

            ref var locations = ref _locations.GetEntity(0).Get<Locations>();
            ref var hoveredCoords = ref _hoveredCoords.GetEntity(0).Get<HoveredCoords>();

            if (hoveredCoords.Coords.Cube != _previousCoords && Input.IsActionPressed("editor_select"))
            {
                _previousCoords = hoveredCoords.Coords.Cube;
                
                var chunks = new List<Vector3i>();

                foreach (var cube in Hex.GetCellsInRange(hoveredCoords.Coords.Cube, editorView.BrushSize))
                {
                    if (!locations.Has(cube))
                    {
                        continue;
                    }

                    var locEntity = locations.Get(cube);
                    EditLocation(editorEntity, locEntity);

                    var chunkCell = locEntity.Get<Vector3i>();

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

                SendUpdateMapEvent(chunks);
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
                ref var overlayTerrainEntity = ref locEntity.Get<HasOverlayTerrain>().Entity;

                if (!overlayTerrainEntity.IsNull())
                {
                    overlayTerrainEntity.Destroy();
                }

                overlayTerrainEntity = editorView.TerrainEntity.Copy();
            }
            else
            {
                baseTerrainEntity.Destroy();
                baseTerrainEntity = editorView.TerrainEntity.Copy();
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
        var updateMapEventEntity = _world.NewEntity();
        updateMapEventEntity.Replace(new UpdateMapEvent(chunks));
    }
}