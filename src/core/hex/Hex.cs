using System.Collections.Generic;
using Godot;

public class Hex
{
    private static readonly Vector3[] NeighborTable = new Vector3[6] {
        new Vector3(0, -1, 1), // NE
        new Vector3(1, -1, 0), // SW
        new Vector3(1, 0, -1), // W
        new Vector3(0, 1, -1), // NW
        new Vector3(-1, 1, 0), // E
        new Vector3(-1, 0, 1), // SE
    };

    public static Vector3 Cube2Axial(Vector3 cube)
    {
        return new Vector3(cube.x, 0, cube.z);
    }

    public static Vector3 Axial2Cube(Vector3 axial)
    {
        return new Vector3(axial.x, -axial.x - axial.z, axial.z);
    }

    public static Vector3 World2Axial(Vector3 position)
    {
        var fx = position.x / (Metrics.InnerRadius * 2.0f);
        var fy = -fx;

        var offset = position.z / (Metrics.OuterRadius * 3.0f);
        fx -= offset;
        fy -= offset;

        var x = Mathf.Round(fx);
        var y = Mathf.Round(fy);
        var z = Mathf.Round(-fx -fy);

        if (x + y + z != 0)
        {
            var dx = Mathf.Abs(fx - x);
            var dy = Mathf.Abs(fy - y);
            var dz = Mathf.Abs(-fx - fy - z);

            if (dx > dy && dx > dz)
            {
                x = -y - z;
            }

            if (dz > dy)
            {
                z = -x - y;
            }
        }

        return new Vector3(x, 0, z);
    }

    public static Vector3 Offset2World(Vector3 offset)
    {
        var x = ((int)offset.x + (int)offset.z * 0.5f - (int)offset.z / 2) * (Metrics.InnerRadius * 2.0f);
        var z = (int)offset.z * Metrics.OuterRadius * 1.5f;
	    return new Vector3(x, 0, z);
    }

    public static Vector3 Axial2Offset(Vector3 axial)
    {
        return new Vector3(axial.x + axial.z / 2, 0, axial.z);
    }

    public static Vector3 Offset2Axial(Vector3 offset)
    {
        return new Vector3(offset.x - offset.z / 2, 0, offset.z);
    }

    public static Vector3 Cube2Offset(Vector3 cube)
    {
        var x = cube.x + (cube.z - ((int)cube.x & 1)) / 2;
        var z = cube.z;
        return new Vector3(x, 0, z);
    }

    public static Vector3 Offset2Cube(Vector3 offset)
    {
        var x = offset.x - (offset.z - ((int)offset.z & 1)) / 2;
        var z = offset.z;
        var y = -x - z;
        return new Vector3(x, y, z);
    }

    public static Vector3 GetNeighbor(Vector3 cube, Direction direction)
    {
        return cube + NeighborTable[(int)direction];
    }

    public static Vector3[] GetNeighbors(Vector3 cube)
    {
        Vector3[] neighbors = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = GetNeighbor(cube, (Direction)i);
        }

        return neighbors;
    }

    public static Vector3[] GetCellsInRange(Vector3 cube, int radius)
    {
        List<Vector3> cells = new List<Vector3>();

        for (int x = -radius; x < radius + 1; x++)
        {
            for (int y = Mathf.Max(-radius, -x - radius); y < Mathf.Min(radius + 1, -x + radius + 1); y++)
            {
                var z = -x - y;
                cells.Add(cube + new Vector3(x, y, z));
            }
        }
        
        return cells.ToArray();
    }
}