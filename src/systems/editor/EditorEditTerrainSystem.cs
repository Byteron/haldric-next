using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class Cell
{
    public Vector3i Value;
    public Cell() => Value = Vector3i.Zero;
    public Cell(Vector3i value) => Value = value;
}

public class EditorEditTerrainSystem : ISystem
{
    Node3D _parent;

    Vector3 _previousCoords;

    public EditorEditTerrainSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<Map>(out var map)) return;

        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation)) return;

        var editorView = commands.GetElement<EditorView>();

        if (editorView.Mode != EditorView.EditorMode.Terrain) return;

        var locEntity = hoveredLocation.Entity;

        if (locEntity is null || !locEntity.IsAlive) return;

        var locations = map.Locations;

        var hoveredCoords = locEntity.Get<Coords>();
        
        if (hoveredCoords.Cube() == _previousCoords || !Input.IsActionPressed("editor_select")) return;
        _previousCoords = hoveredCoords.Cube();

        var chunks = new List<Vector3i>();

        foreach (var cube in Hex.GetCellsInRange(hoveredCoords.Cube(), editorView.BrushSize))
        {
            if (!locations.Has(cube))
            {
                continue;
            }

            var nLocEntity = locations.Get(cube);
            EditLocation(editorView, nLocEntity);

            var chunkCell = nLocEntity.Get<Cell>().Value;

            if (chunks.Contains(chunkCell)) continue;
            
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

        if (!editorView.UseTerrain && !editorView.UseElevation)
        {
            return;
        }

        if (editorView.TerrainEntity.Has<HasOverlayTerrain>())
        {
            commands.Send(new UpdateTerrainFeaturePopulatorEvent(chunks));
        }
        else
        {
            commands.Send(new UpdateMapTrigger(chunks));
        }
    }

     void EditLocation(EditorView editorView, Entity locEntity)
    {
        var baseTerrain = locEntity.Get<HasBaseTerrain>();
        var elevation = locEntity.Get<Elevation>();

        if (editorView.UseTerrain)
        {
            if (editorView.TerrainEntity.Has<IsOverlayTerrain>())
            {
                if (!locEntity.Has<HasOverlayTerrain>())
                {
                    locEntity.Add<HasOverlayTerrain>();
                }

                if (Input.IsActionPressed("editor_no_base"))
                {
                    locEntity.Get<HasOverlayTerrain>().Entity = editorView.TerrainEntity;
                }
                else
                {
                    var code = editorView.TerrainEntity.Get<TerrainCode>().Value;

                    if (Data.Instance.DefaultOverlayBaseTerrains.ContainsKey(code))
                    {
                        var baseCode = Data.Instance.DefaultOverlayBaseTerrains[code];
                        var baseTerrainEntity = Data.Instance.Terrains[baseCode];
                        locEntity.Get<HasBaseTerrain>().Entity = baseTerrainEntity;
                    }

                    locEntity.Get<HasOverlayTerrain>().Entity = editorView.TerrainEntity;
                }
            }
            else
            {
                if (locEntity.Has<HasOverlayTerrain>())
                {
                    locEntity.Remove<HasOverlayTerrain>();
                }

                baseTerrain.Entity = editorView.TerrainEntity;
            }
        }

        if (editorView.UseElevation)
        {
            elevation.Value = editorView.Elevation;
        }
    }
}