using System.Collections.Generic;
using Godot;

public class Hex
{
    private static readonly Vector3[] _neighborTable = new Vector3[6]
    {
        new Vector3(0, -1, 1), // E
        new Vector3(1, -1, 0), // SE
        new Vector3(1, 0, -1), // SW
        new Vector3(0, 1, -1), // W
        new Vector3(-1, 1, 0), // NW
        new Vector3(-1, 0, 1), // NE
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
        float fx = position.x / (Metrics.InnerRadius * 2.0f);
        float fy = -fx;

        float offset = position.z / (Metrics.OuterRadius * 3.0f);
        fx -= offset;
        fy -= offset;

        float x = Mathf.Round(fx);
        float y = Mathf.Round(fy);
        float z = Mathf.Round(-fx - fy);

        if (x + y + z != 0)
        {
            float dx = Mathf.Abs(fx - x);
            float dy = Mathf.Abs(fy - y);
            float dz = Mathf.Abs(-fx - fy - z);

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
        float zOffset = System.Math.Abs(offset.z);
        float x = ((int)offset.x + (int)zOffset * 0.5f - (int)zOffset / 2) * (Metrics.InnerRadius * 2.0f);
        float z = (int)offset.z * Metrics.OuterRadius * 1.5f;
        return new Vector3(x, 0, z);
    }

    public static Vector3 Axial2Offset(Vector3 axial)
    {
        Vector3 cube = Axial2Cube(axial);
        Vector3 offset = Cube2Offset(cube);
        return offset;
    }

    public static Vector3 Offset2Axial(Vector3 offset)
    {
        Vector3 cube = Offset2Cube(offset);
        Vector3 axial = Cube2Axial(cube);
        return axial;
    }

    public static Vector3 Cube2Offset(Vector3 cube)
    {
        cube.x += (cube.z - ((int)cube.z & 1)) / 2;
        cube.y = 0;
        return cube;
    }

    public static Vector3 Offset2Cube(Vector3 offset)
    {
        offset.x -= (offset.z - ((int)offset.z & 1)) / 2;
        offset.y = -offset.x - offset.z;
        return offset;
    }

    public static Vector3 GetNeighbor(Vector3 cube, Direction direction)
    {
        return cube + _neighborTable[(int)direction];
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
                float z = -x - y;
                cells.Add(cube + new Vector3(x, y, z));
            }
        }

        return cells.ToArray();
    }

    public static int GetDistance(Vector3 cube1, Vector3 cube2)
    {
        return (int)Mathf.Max(Mathf.Abs(cube1.x - cube2.x), Mathf.Max(Mathf.Abs(cube1.y - cube2.y), Mathf.Abs(cube1.z - cube2.z)));
    }

    public static Vector3[] GetLine(Vector3 cube1, Vector3 cube2)
    {
        List<Vector3> list = new List<Vector3>();

        int distance = GetDistance(cube1, cube2);

        for (int i = 0; i < distance; i++)
        {
            list.Add(cube1.Lerp(cube2, 1.0f / distance * i).Round());
        }

        return list.ToArray();
    }
}