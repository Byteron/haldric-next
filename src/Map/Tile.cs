﻿using Godot;

namespace Haldric;

public partial class Tile : RefCounted
{
    public int Index;

    public Coords Coords;
    public readonly Neighbors Neighbors = new();

    public Vector2I ChunkCell;

    public float BlendFactor = 0.25f;
    public float SolidFactor = 0.75f;

    public int Elevation;

    public Terrain BaseTerrain = default!;
    public Terrain? OverlayTerrain;

    public int Distance;
    public Coords PathFrom;
}

public class Neighbors
{
    public readonly Tile?[] Array = new Tile[6];

    public Tile? Get(Direction direction)
    {
        return Array[(int)direction];
    }

    public void Set(Direction direction, Tile tile)
    {
        Array[(int)direction] = tile;
    }
}