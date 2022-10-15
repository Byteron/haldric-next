using Godot;
using RelEcs;

public static class EditorSystems
{
    static Coords _previousCoords;

    public static void SpawnEditorMenu(this World world)
    {
        var tree = world.GetTree();
        var data = world.GetElement<TerrainData>();

        var view = Scenes.Instantiate<EditorView>();
        view.CreateButton.Pressed += world.OnCreateButtonPressed;
        view.LoadButton.Pressed += world.OnLoadButtonPressed;
        view.SaveButton.Pressed += world.OnSaveButtonPressed;
        view.ToolsTab.TabChanged += world.OnToolsTabChanged;

        world.AddElement(view);
        tree.CurrentScene.AddChild(view);


        foreach (var (code, entity) in data.TerrainEntities)
        {
            var button = new Button();
            button.CustomMinimumSize = new Vector2i(50, 50);
            button.Text = code;
            button.Pressed += () => world.GetElement<SelectedTerrain>().Entity = entity;
            view.Terrains.AddChild(button);
        }
    }

    public static void DespawnEditorMenu(this World world)
    {
        var menu = world.GetElement<EditorView>();
        world.RemoveElement<EditorView>();
        menu.QueueFree();
    }

    static void OnToolsTabChanged(this World world, long index)
    {
        var view = world.GetElement<EditorView>();

        view.Mode = index switch
        {
            0 => EditorMode.Terrain,
            1 => EditorMode.Player,
            _ => view.Mode
        };
    }

    static void OnCreateButtonPressed(this World world)
    {
        var view = world.GetElement<EditorView>();

        if (view.WidthTextEdit.Text.IsValidInteger() && view.HeightTextEdit.Text.IsValidInteger())
        {
            var width = int.Parse(view.WidthTextEdit.Text);
            var height = int.Parse(view.HeightTextEdit.Text);

            world.Enqueue(new CreatMapCommand
            {
                Width = width,
                Height = height,
            });
        }
        else
        {
            GD.PushWarning("Please specify valid map size!");
        }
    }

    static void OnSaveButtonPressed(this World world)
    {
        var view = world.GetElement<EditorView>();

        if (string.IsNullOrEmpty(view.MapNameTextEdit.Text))
        {
            GD.PushWarning("Invalid Map Name: Please specify a Map Name");
        }
        else if (view.MapNameTextEdit.Text.IsValidIdentifier())
        {
            world.Enqueue(new SaveMapCommand
            {
                MapName = view.MapNameTextEdit.Text,
            });
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }

    static void OnLoadButtonPressed(this World world)
    {
        var view = world.GetElement<EditorView>();

        if (string.IsNullOrEmpty(view.MapNameTextEdit.Text))
        {
            GD.PushWarning("Invalid Map Name: Please specify a Map Name");
        }
        else if (view.MapNameTextEdit.Text.IsValidIdentifier())
        {
            world.Enqueue(new LoadMapCommand
            {
                MapName = view.MapNameTextEdit.Text,
            });
        }
        else
        {
            GD.PushWarning("Invalid Map Name: Not a Valid Identifier");
        }
    }

    public static void EditorEditPlayer(this World world)
    {
        if (!world.TryGetElement<Map>(out var map)) return;
        if (!world.TryGetElement<SelectedTerrain>(out var selectedTerrain)) return;
        if (!world.TryGetElement<HoveredTile>(out var hoveredTile)) return;

        var view = world.GetElement<EditorView>();
        var scene = world.GetCurrentScene();

        if (view.Mode != EditorMode.Player) return;

        var tileEntity = hoveredTile.Entity;

        if (!world.IsAlive(tileEntity)) return;

        var tiles = world.Query<Coords, Elevation, BaseTerrainSlot>();
        var (coords, elevation, baseTerrainSlot) = tiles.Get(tileEntity);

        if (coords == _previousCoords || !Input.IsActionPressed("editor_select")) return;

        _previousCoords = coords;

        var elevationOffset = world.GetComponent<Elevation>(baseTerrainSlot.Entity);

        if (world.HasComponent<IsStartingPositionOfSide>(tileEntity))
        {
            var flagView = world.GetComponent<FlagView>(tileEntity);

            scene.RemoveChild(flagView);
            flagView.QueueFree();

            world.On(tileEntity).Remove<FlagView>().Remove<IsStartingPositionOfSide>();
            view.RemovePlayer(coords);
        }
        else
        {
            var flagView = Scenes.Instantiate<FlagView>();
            scene.AddChild(flagView);
            var pos = coords.ToWorld();
            pos.y = elevation.Height + elevationOffset.Value;
            flagView.Position = pos;

            world.On(tileEntity)
                .Add(flagView)
                .Add(new IsStartingPositionOfSide { Value = view.Players.Count });

            view.AddPlayer(coords);
        }
    }

    public static void EditorEditTile(this World world)
    {
        if (!world.TryGetElement<Map>(out var map)) return;
        if (!world.TryGetElement<HoveredTile>(out var hoveredTile)) return;
        if (!world.TryGetElement<SelectedTerrain>(out var selectedTerrain)) return;

        var view = world.GetElement<EditorView>();

        if (view.Mode != EditorMode.Terrain) return;

        var tileEntity = hoveredTile.Entity;

        if (tileEntity is null || !world.IsAlive(tileEntity)) return;

        var tiles = map.Tiles;

        var coords = world.GetComponent<Coords>(tileEntity);

        if (coords == _previousCoords || !Input.IsActionPressed("editor_select")) return;

        _previousCoords = coords;

        // var chunks = new List<Vector3i>();

        foreach (var cube in Hex.GetCellsInRange(coords.ToCube(), view.BrushSize))
        {
            if (!tiles.Has(cube)) continue;

            var nTileEntity = tiles.Get(cube);
            world.EditLocation(view, nTileEntity, selectedTerrain.Entity);

            var chunkEntity = world.GetTarget<TileOf>(nTileEntity);
            var chunk = world.GetComponent<Chunk>(chunkEntity);
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

        if (world.HasComponent<OverlayTerrainSlot>(selectedTerrain.Entity))
        {
            world.UpdateTerrainProps();
        }
        else
        {
            world.UpdateTerrainGraphics();
        }
    }

    static void EditLocation(this World world, EditorView editorView, Entity tileEntity, Entity selectedTerrainEntity)
    {
        var data = world.GetElement<TerrainData>();
        var tiles = world.Query<BaseTerrainSlot, OverlayTerrainSlot, Elevation>();
        var codes = world.Query<TerrainCode>();

        var (baseTerrain, overlayTerrain, elevation) = tiles.Get(tileEntity);

        if (editorView.UseTerrain)
        {
            if (world.IsAlive(overlayTerrain.Entity))
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