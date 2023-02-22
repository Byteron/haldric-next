namespace Haldric;

public class MapData
{
    public int Width;
    public int Height;
    public MapDataPlayer[] Players;
    public MapDataLocation[] Locations;
    
    public static MapData Create(int width, int height)
    {
        var mapData = new MapData
        {
            Width = width,
            Height = height,
            Locations = new MapDataLocation[width * height]
        };

        for (var z = 0; z < height; z++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = z * width + x;
                var coords = Coords.FromOffset(x, z);

                var tileData = new MapDataLocation
                {
                    Coords = coords,
                    Terrain = new[] { "Gg" },
                };

                mapData.Locations[index] = tileData;
            }
        }

        return mapData;
    }
}

public struct MapDataPlayer
{
    public Coords Coords;
    public int Side;
}

public struct MapDataLocation
{
    public Coords Coords;
    public string[] Terrain;
    public int Elevation;
}