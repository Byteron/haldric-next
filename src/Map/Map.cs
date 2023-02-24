using Godot;
using Godot.Collections;

namespace Haldric;

public partial class Map : Node3D
{
    [Signal]
    public delegate void TileHoveredEventHandler(Tile tile);
    
    Grid _grid;
    public Coords HoveredCoords { get; private set; }
    public Tile HoveredTile => _tiles[HoveredCoords];

    readonly System.Collections.Generic.Dictionary<Coords, Tile> _tiles = new();

    [Export] Chunks _chunks = default!;

    public override void _Process(double delta)
    {
        UpdateHoveredCoords();
    }

    public void Initialize(int width, int height)
    {
        Initialize(MapData.Create(width, height));
    }
    
    public void Initialize(MapData mapData)
    {
        if (mapData.Locations.Length == 0)
        {
            GD.PushWarning("Cannot initialize map, no tiles present in MapData");
            return;
        }

        _grid = new Grid
        {
            Width = mapData.Width,
            Height = mapData.Height,
        };

        foreach (var tileData in mapData.Locations)
        {
            var baseCode = tileData.Terrain[0];
            var baseTerrain = Data.Instance.Terrains[baseCode];
            Terrain? overlayTerrain = null;

            if (tileData.Terrain.Length == 2)
            {
                var overlayCode = tileData.Terrain[1];
                overlayTerrain = Data.Instance.Terrains[overlayCode];
            }

            var offset = tileData.Coords.ToOffset();
            var index = (int)offset.Y * _grid.Width + (int)offset.X;
            var tile = new Tile
            {
                Index = index,
                Coords = tileData.Coords,
                Elevation = tileData.Elevation,
                BaseTerrain = baseTerrain,
                OverlayTerrain = overlayTerrain,
            };
            
            _tiles.Add(tileData.Coords, tile);
        }

        foreach (var (coords, tile) in _tiles)
        {
            foreach (var (direction, nCoords) in coords.GetNeighbors())
            {
                if (!_tiles.TryGetValue(nCoords, out var nTile)) continue;
                tile.Neighbors.Set(direction, nTile);
            }
        }

        _chunks.Initialize(_grid, _tiles);
        _chunks.UpdateTerrainMeshes(_tiles);
        _chunks.UpdateTerrainProps(_tiles);
    }

    public Tile GetTile(Coords coords)
    {
        return _tiles[coords];
    }

    void UpdateHoveredCoords()
    {
        var result = ShootRay();

        if (!result.ContainsKey("position")) return;

        var position = (Vector3)result["position"];
        var coords = Coords.FromWorld(position);

        if (HoveredCoords == coords) return;
        if (!_tiles.TryGetValue(coords, out var tile)) return;
        
        HoveredCoords = coords;
        
        GD.Print(HoveredCoords);
        EmitSignal(SignalName.TileHovered, tile);
    }

    Dictionary ShootRay()
    {
        var scene = (Node3D)GetTree().CurrentScene;
        var spaceState = scene.GetWorld3D().DirectSpaceState;
        var viewport = scene.GetViewport();

        var camera = viewport.GetCamera3D();
        var mousePosition = viewport.GetMousePosition();

        if (camera == null) return new Dictionary();

        var from = camera.ProjectRayOrigin(mousePosition);
        var to = from + camera.ProjectRayNormal(mousePosition) * 1000f;

        var parameters3D = new PhysicsRayQueryParameters3D();
        parameters3D.From = from;
        parameters3D.To = to;

        return spaceState.IntersectRay(parameters3D);
    }
}

public struct Grid
{
    public int Width;
    public int Height;

    public bool IsCoordsInGrid(Coords coords)
    {
        return coords.ToOffset().X > -1 && coords.ToOffset().X < Width && coords.ToOffset().Z > -1 &&
               coords.ToOffset().Z < Height;
    }

    public override string ToString()
    {
        return $"({Width}, {Height})";
    }
}