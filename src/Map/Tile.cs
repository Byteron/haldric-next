using System.Collections.Generic;
using Godot;

namespace Haldric;

public partial class Tile : RefCounted
{
    public int Index;

    public Coords Coords;
    
    public readonly Tile?[] Neighbors = new Tile[6];
    
    public List<Tile>? Castle = null;
    public List<Tile>? Village = null;
    
    public Chunk Chunk = default!;

    public float BlendFactor = 0.25f;
    public float SolidFactor = 0.75f;

    public int Elevation;

    public Terrain BaseTerrain = default!;
    public Terrain? OverlayTerrain;

    public Unit? Unit;

    public Tile? PathFromTile;
    public int Distance;
    public bool IsInZoc;

    public Vector3 WorldPosition => Coords.ToWorld() + new Vector3(0, Elevation * Metrics.ElevationStep, 0);
}