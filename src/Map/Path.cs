using System.Collections.Generic;

namespace Haldric;

public struct Path
{
    public Tile StartTile;
    public Tile EndTile;

    public Queue<Tile> Checkpoints;

    public override string ToString()
    {
        return $"From: {StartTile.Coords}, To: {EndTile.Coords}, Checkpoints: {Checkpoints.Count}";
    }
}