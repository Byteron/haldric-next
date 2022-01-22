using System.Collections.Generic;
using Bitron.Ecs;

public class MapDataPlayer
{
    public Coords Coords;
    public int Side;
}

public class MapDataLocation
{
    public Coords Coords;
    public List<string> Terrain = new List<string>();
    public int Elevation;
}

public class MapData
{
    public int Width = 0;
    public int Height = 0;
    public List<MapDataPlayer> Players = new List<MapDataPlayer>();
    public List<MapDataLocation> Locations = new List<MapDataLocation>();
}