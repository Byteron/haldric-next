using System.Collections.Generic;
using Godot;

namespace Haldric;

public static class Hex
{
    static readonly Vector3[] NeighborTable =
    {
        new(-1, 1, 0), // E
        new(0, 1, -1), // SE
        new(1, 0, -1), // SW
        new(1, -1, 0), // W
        new(0, -1, 1), // NW
        new(-1, -0, 1), // NE
    };

    public static Vector3 Cube2Axial(Vector3 cube)
    {
        //return new Vector3(cube.x, 0, cube.z);
        cube.Y = 0;
        return cube;
    }

    public static Vector3 Axial2Cube(Vector3 axial)
    {
        //return new Vector3(axial.x, -axial.x - axial.z, axial.z);
        axial.Y = -axial.X - axial.Z;
        return axial;
    }

    public static Vector3 World2Axial(Vector3 position)
    {
        var fx = position.X / (Metrics.InnerRadius * 2.0f);
        var fy = -fx;

        var offset = position.Z / (Metrics.OuterRadius * 3.0f);
        fx -= offset;
        fy -= offset;

        var x = Mathf.Round(fx);
        var y = Mathf.Round(fy);
        var z = Mathf.Round(-fx - fy);

        if (x + y + z == 0) return new Vector3(x, 0, z);

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

        return new Vector3(x, 0, z);
    }

    public static int Offset2Id(Vector3 offset, int width)
    {
        var x = (int)offset.X;
        var z = (int)offset.Z;

        return z * width + x;
    }

    public static int Axial2Id(Vector3 axial, int width)
    {
        var cube = Axial2Cube(axial);
        var offset = Cube2Offset(cube);

        return Offset2Id(offset, width);
    }

    public static int Cube2Id(Vector3 cube, int width)
    {
        var offset = Cube2Offset(cube);
        return Offset2Id(offset, width);
    }

    public static Vector3 Offset2World(Vector3 offset)
    {
        var zOffset = System.Math.Abs(offset.Z);
        var x = ((int)offset.X + (int)zOffset * 0.5f - (int)zOffset / 2) * (Metrics.InnerRadius * 2.0f);
        var z = (int)offset.Z * Metrics.OuterRadius * 1.5f;
        return new Vector3(x, 0, z);
    }

    public static Vector3 Axial2Offset(Vector3 axial)
    {
        var cube = Axial2Cube(axial);
        var offset = Cube2Offset(cube);
        return offset;
    }

    public static Vector3 Axial2World(Vector3 axial)
    {
        var offset = Axial2Offset(axial);
        var world = Offset2World(offset);
        return world;
    }

    public static Vector3 Offset2Axial(Vector3 offset)
    {
        var cube = Offset2Cube(offset);
        var axial = Cube2Axial(cube);
        return axial;
    }

    public static Vector3 Cube2Offset(Vector3 cube)
    {
        cube.X += (cube.Z - ((int)cube.Z & 1)) / 2;
        cube.Y = 0;
        return cube;
    }

    public static Vector3 Offset2Cube(Vector3 offset)
    {
        offset.X -= (offset.Z - ((int)offset.Z & 1)) / 2;
        offset.Y = -offset.X - offset.Z;
        return offset;
    }

    public static Vector3 GetNeighbor(Vector3 cube, Direction direction)
    {
        return cube + NeighborTable[(int)direction];
    }

    public static Vector3[] GetNeighbors(Vector3 cube)
    {
        var neighbors = new Vector3[6];

        for (var i = 0; i < 6; i++)
        {
            neighbors[i] = GetNeighbor(cube, (Direction)i);
        }

        return neighbors;
    }

    public static Vector3[] GetCellsOfRing(Vector3 cube, int radius)
    {
        var cells = new List<Vector3>();

        var currentCell = NeighborTable[4] * radius;
        for (var i = 0; i < 6; i++)
        {
            for (var r = 0; r < radius; r++)
            {
                currentCell += NeighborTable[i];
                cells.Add(cube + currentCell);
            }
        }

        return cells.ToArray();
    }

    public static Vector3[] GetBorderOfRange(Vector3 cube, int radius)
    {
        var points = new List<Vector3>();

        var cellsSegments = radius * 2;

        for (var i = 0; i < 3; i++)
        {
            var cornerOffsetA = Metrics.Corners[i * 2];
            var cornerOffsetB = Metrics.Corners[i * 2 + 1];

            var dirA = Direction.Ne + i * 2;

            var directionA = NeighborTable[(int)dirA];
            var dirB = dirA.Next2();
            var directionB = NeighborTable[(int)dirB];

            for (var r = 0; r <= cellsSegments; r++)
            {
                var multiA = Mathf.Min(cellsSegments - r, radius);
                var multiB = Mathf.Min(r, radius);
                var currentCube = directionA * multiA + directionB * multiB;
                var currentOffset = Cube2Offset(currentCube + cube);
                var worldPos = Offset2World(currentOffset);

                points.Add(worldPos + cornerOffsetA);
                points.Add(worldPos + cornerOffsetB);
            }
        }

        return points.ToArray();
    }

    public static Vector3[] GetCellsInRange(Vector3 cube, int radius)
    {
        var cells = new List<Vector3>();

        for (var x = -radius; x < radius + 1; x++)
        {
            for (var y = Mathf.Max(-radius, -x - radius); y < Mathf.Min(radius + 1, -x + radius + 1); y++)
            {
                float z = -x - y;
                cells.Add(cube + new Vector3(x, y, z));
            }
        }

        return cells.ToArray();
    }

    public static int GetDistance(Vector3 cube1, Vector3 cube2)
    {
        var dx = Mathf.Abs(cube1.X - cube2.X);
        var dy = Mathf.Abs(cube1.Y - cube2.Y);
        var dz = Mathf.Abs(cube1.Z - cube2.Z);
        return (int)Mathf.Max(dx, Mathf.Max(dy, dz));
    }
}