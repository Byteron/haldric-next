using Godot;

namespace Haldric;

public partial class Editor : Node3D
{
    Map _map = default!;

    [Export] PackedScene _mapScene = default!;
    [Export] CameraOperator _cameraOperator = default!;
    [Export] EditorHud _hud = default!;

    Coords _prevCoords = Coords.Invalid;

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            GetTree().ChangeSceneToFile("res://src/Menu/MainMenu.tscn");
        }
    }

    public override void _Ready()
    {
        _map = _mapScene.Instantiate<Map>();
        AddChild(_map);

        _map.Initialize(40, 40);
    }

    public override void _Process(double delta)
    {
        if (!Input.IsActionPressed("editor_select") || _map.HoveredTile.Coords == _prevCoords) return;

        _prevCoords = _map.HoveredTile.Coords;

        foreach (var coords in _map.HoveredTile.Coords.GetCoordsInRange(_hud.BrushSize))
        {
            if (!_map.HasTile(coords)) continue;
            var tile = _map.GetTile(coords);
            EditTile(tile);
            tile.Chunk.IsDirty = true;

            foreach (var nTile in _map.HoveredTile.Neighbors)
            {
                if (nTile is null) continue;
                nTile.Chunk.IsDirty = true;
            }
        }

        if (_hud is { UseTerrain: false, UseElevation: false }) return;

        _map.UpdateTerrain();
    }

    void EditTile(Tile tile)
    {
        if (_hud.UseTerrain)
        {
            if (_hud.SelectedTerrain.IsBase)
            {
                tile.OverlayTerrain = null;
                tile.BaseTerrain = _hud.SelectedTerrain;
            }
            else
            {
                if (Input.IsActionPressed("editor_no_base"))
                {
                    tile.OverlayTerrain = _hud.SelectedTerrain;
                }
                else
                {
                    var code = _hud.SelectedTerrain.DefaultBase;
                    code ??= string.Empty;
                    if (Data.Instance.Terrains.TryGetValue(code, out var baseTerrain))
                    {
                        tile.BaseTerrain = baseTerrain;
                    }

                    tile.OverlayTerrain = _hud.SelectedTerrain;
                }
            }
        }

        if (_hud.UseElevation) tile.Elevation = _hud.Elevation;
    }

    void OnCreateButtonPressed(int width, int height)
    {
        CreateNewMap(width, height);
    }

    void OnSaveButtonPressed(string name)
    {
        // TODO: Save Map
    }

    void OnLoadButtonPressed(string name)
    {
        // TODO: Load Map
    }

    void CreateNewMap(int width, int height)
    {
        _map?.QueueFree();
        _map = _mapScene.Instantiate<Map>();
        AddChild(_map);

        _map.Initialize(width, height);
    }
}