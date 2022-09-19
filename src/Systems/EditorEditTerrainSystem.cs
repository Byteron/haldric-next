using Godot;
using RelEcs;

public class EditorEditTerrainSystem : ISystem
{
    Coords _previousCoords;

    public World World { get; set; }

    public void Run()
    {
        if (!this.TryGetElement<Map>(out var map)) return;
        if (!this.TryGetElement<HoveredTile>(out var hoveredTile)) return;
        if (!this.TryGetElement<SelectedTerrain>(out var selectedTerrain)) return;

        var view = this.GetElement<EditorView>();

        if (view.Mode != EditorMode.Terrain) return;

        var tileEntity = hoveredTile.Entity;

        if (tileEntity is null || !this.IsAlive(tileEntity)) return;

        var tiles = map.Tiles;

        var coords = this.GetComponent<Coords>(tileEntity);
        
        if (coords == _previousCoords || !Input.IsActionPressed("editor_select")) return;
        
        _previousCoords = coords;

        // var chunks = new List<Vector3i>();

        foreach (var cube in Hex.GetCellsInRange(coords.ToCube(), view.BrushSize))
        {
            if (!tiles.Has(cube)) continue;

            var nTileEntity = tiles.Get(cube);
            EditLocation(view, nTileEntity, selectedTerrain.Entity);
            
            var chunkEntity = this.GetTarget<TileOf>(nTileEntity);
            var chunk = this.GetComponent<Chunk>(chunkEntity);
            chunk.IsDirty = true;
            
            // TODO: Mark neighboring chunks as dirty too
            // chunks.Add(chunkCell);
            // chunks.Add(chunkCell + new Vector3i(1, 0, 1));
            // chunks.Add(chunkCell + new Vector3i(1, 0, 0));
            // chunks.Add(chunkCell + new Vector3i(1, 0, -1));
            // chunks.Add(chunkCell + new Vector3i(-1, 0, 1));
            // chunks.Add(chunkCell + new Vector3i(-1, 0, 0));
            // chunks.Add(chunkCell + new Vector3i(-1, 0, -1));
            // chunks.Add(chunkCell + new Vector3i(0, 0, 1));
            // chunks.Add(chunkCell + new Vector3i(0, 0, -1));
        }

        if (!view.UseTerrain && !view.UseElevation) return;

        if (this.HasComponent<OverlayTerrainSlot>(selectedTerrain.Entity))
        {
            this.UpdateTerrainProps();
        }
        else
        {
            this.UpdateTerrainGraphics();
        }
    }

    void EditLocation(EditorView editorView, Entity tileEntity, Entity selectedTerrainEntity)
    {
        var data = this.GetElement<TerrainData>();
        var tiles = this.Query<BaseTerrainSlot, OverlayTerrainSlot, Elevation>();
        var codes = this.Query<TerrainCode>();

        var (baseTerrain, overlayTerrain, elevation) = tiles.Get(tileEntity);

        if (editorView.UseTerrain)
        {
            if (this.IsAlive(overlayTerrain.Entity))
            {
                if (Input.IsActionPressed("editor_no_base"))
                {
                    overlayTerrain.Entity = selectedTerrainEntity;
                }
                else
                {
                    var code = codes.Get(selectedTerrainEntity);

                    if (data.DefaultOverlayBaseTerrains.ContainsKey(code.Value))
                    {
                        var baseCode = data.DefaultOverlayBaseTerrains[code.Value];
                        var baseTerrainEntity = data.TerrainEntities[baseCode];
                        baseTerrain.Entity = baseTerrainEntity;
                    }

                    overlayTerrain.Entity = selectedTerrainEntity;
                }
            }
            else
            {
                overlayTerrain.Entity = Entity.None;
                baseTerrain.Entity = selectedTerrainEntity;
            }
        }

        if (editorView.UseElevation) elevation.Value = editorView.Elevation;
    }
}