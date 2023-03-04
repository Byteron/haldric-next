using System.Collections.Generic;
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
    [Export] Node3D _cursor = default!;
    
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

        // Initialize Neighbors
        foreach (var (coords, tile) in _tiles)
        {
            foreach (var (direction, nCoords) in coords.GetNeighbors())
            {
                if (!_tiles.TryGetValue(nCoords, out var nTile)) continue;
                tile.Neighbors[(int)direction] = nTile;
            }
        }

        // Initialize Castles
        foreach (var tile in _tiles.Values)
        {
            var canRecruitFrom = tile.OverlayTerrain is null
                ? tile.BaseTerrain.CanRecruitFrom
                : tile.BaseTerrain.CanRecruitFrom || tile.OverlayTerrain.CanRecruitFrom;
            
            if (!canRecruitFrom) continue;
            tile.Castle = FindConnectedCastleTiles(tile);
            GD.Print($"Castle: {tile.Castle.Count}");
        }

        // Initialize Villages
        foreach (var tile in _tiles.Values)
        {
            var isCapturable = tile.OverlayTerrain is null
                ? tile.BaseTerrain.IsCapturable
                : tile.BaseTerrain.IsCapturable || tile.OverlayTerrain.IsCapturable;
            
            if (!isCapturable) continue;
            
            tile.Village = FindConnectedVillageTiles(tile);
            GD.Print($"Village: {tile.Village.Count}");
        }

        _chunks.Initialize(_grid, _tiles);
        
        UpdateTerrain();
    }

    public Tile GetTile(Coords coords)
    {
        return _tiles[coords];
    }

    public bool HasTile(Coords coords)
    {
        return _tiles.ContainsKey(coords);
    }

    public void UpdateTerrain()
    {
        _chunks.UpdateTerrainMeshes(_tiles);
        _chunks.UpdateTerrainProps(_tiles);
        _chunks.CleanDirtyChunks();
    }

    public void UpdateTerrainPropsOnly()
    {
        _chunks.UpdateTerrainProps(_tiles);
        _chunks.CleanDirtyChunks();
    }

    // We only need the end coords, because the start position is known through UpdatePathInfo
    public void MoveUnit(Coords endCoords)
    {
        var maybePath = FindPath(endCoords);

        if (maybePath is not { } path) return;

        var startTile = path.StartTile;
        var endTile = path.EndTile;

        if (startTile.Unit is not { } unit) return;
        if (endTile.Unit is not null) return;

        startTile.Unit = null;
        endTile.Unit = unit;

        var tween = GetTree().CreateTween();

        foreach (var tile in path.Checkpoints)
        {
            var pos = tile.WorldPosition;
            pos.Y += tile.BaseTerrain.ElevationOffset;

            tween.TweenCallback(Callable.From(() =>
            {
                unit.LookAt(pos, Vector3.Up);
                var rot = unit.Rotation;
                rot.X = 0;
                rot.Z = 0;
                unit.Rotation = rot;
            }));

            tween.SetTrans(Tween.TransitionType.Linear)
                .TweenProperty(unit, "position", pos, 0.25f);
        }
    }

    public void UpdatePathInfo(Coords fromCoords, int side)
    {
        // Clear Path Info
        foreach (var tile in _tiles.Values)
        {
            tile.PathFromTile = null;
            tile.Distance = int.MaxValue;
            tile.IsInZoc = false;
        }

        // Update ZoC
        foreach (var tile in _tiles.Values)
        {
            if (tile.Unit is not { } unit) continue;

            if (unit.Side == side) continue;

            foreach (var nTile in tile.Neighbors)
            {
                if (nTile is null || nTile.IsInZoc || nTile.Unit is not null) continue;
                nTile.IsInZoc = true;
            }
        }

        var fromTile = _tiles[fromCoords];
        fromTile.Distance = 0;

        var frontier = new List<Tile> { fromTile };

        while (frontier.Count > 0)
        {
            var cTile = frontier[0];
            frontier.RemoveAt(0);

            var cCoords = cTile.Coords;

            var cDistance = cTile.Distance;
            var cElevation = cTile.Elevation;

            foreach (var nTile in cTile.Neighbors)
            {
                if (nTile is null) continue;

                var nElevation = nTile.Elevation;
                var elevationDifference = Mathf.Abs(cElevation - nElevation);

                if (elevationDifference > 1) continue;

                var nMovementCost = 1;

                if (cTile.Unit is { } unit && cCoords != fromCoords)
                {
                    if (unit.Side != side) nMovementCost = 99;
                }

                if (cTile.IsInZoc) nMovementCost = 99;

                var distance = cDistance + nMovementCost;

                if (distance < nTile.Distance)
                {
                    if (nTile.Distance == int.MaxValue) frontier.Add(nTile);
                    nTile.Distance = distance;
                    nTile.PathFromTile = cTile;
                }

                frontier.Sort((locA, locB) => locA.Distance.CompareTo(locB.Distance));
            }
        }
    }

    public Path? FindPath(Coords endCoords)
    {
        var endTile = _tiles[endCoords];

        if (endTile.PathFromTile is null) return null;

        var stack = new Stack<Tile>();
        stack.Push(endTile);
        stack.Push(endTile.PathFromTile);

        while (true)
        {
            var tile = stack.Peek();
            if (tile.PathFromTile is null) break;
            stack.Push(tile.PathFromTile);
        }

        return new Path
        {
            StartTile = stack.Pop(),
            EndTile = endTile,
            Checkpoints = new Queue<Tile>(stack),
        };
    }

    static List<Tile> FindConnectedCastleTiles(Tile tile)
    {
        var list = new List<Tile>();
        var frontier = new Queue<Tile>();
        frontier.Enqueue(tile);

        while (frontier.Count > 0)
        {
            var cTile = frontier.Dequeue();
            var cNeighbors = cTile.Neighbors;

            foreach (var nTile in cNeighbors)
            {
                if (nTile is null) continue;
                if (list.Contains(nTile)) continue;

                var nBaseTerrain = nTile.BaseTerrain;

                var canRecruitTo = nBaseTerrain.CanRecruitTo;

                if (nTile.OverlayTerrain is { } nOverlayTerrain)
                {
                    canRecruitTo = canRecruitTo || nOverlayTerrain.CanRecruitTo;
                }

                if (!canRecruitTo) continue;

                frontier.Enqueue(nTile);
                list.Add(nTile);
            }
        }

        return list;
    }

    static List<Tile> FindConnectedVillageTiles(Tile tile)
    {
        var list = new List<Tile>();
        var frontier = new Queue<Tile>();
        frontier.Enqueue(tile);

        while (frontier.Count > 0)
        {
            var cTile = frontier.Dequeue();
            var cNeighbors = cTile.Neighbors;

            foreach (var nTile in cNeighbors)
            {
                if (nTile is null) continue;
                if (list.Contains(nTile)) continue;

                var nBaseTerrain = nTile.BaseTerrain;

                var givesIncome = nBaseTerrain.GivesIncome;

                if (nTile.OverlayTerrain is { } nOverlayTerrain)
                {
                    givesIncome = givesIncome || nOverlayTerrain.GivesIncome;
                }

                if (!givesIncome) continue;

                frontier.Enqueue(nTile);
                list.Add(nTile);
            }
        }

        return list;
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
        _cursor.Position = tile.WorldPosition;
        
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
        var offset = coords.ToOffset();
        return offset.X > -1 && offset.X < Width && offset.Z > -1 && offset.Z < Height;
    }

    public override string ToString()
    {
        return $"({Width}, {Height})";
    }
}