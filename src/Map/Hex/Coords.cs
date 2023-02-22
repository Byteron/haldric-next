using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Haldric;

public struct Coords
{
    public static Coords Zero = FromXZ(0, 0);
    public static Coords Invalid = FromXZ(int.MaxValue, int.MaxValue);

    public int X;
    public int Z;

    public static Coords FromXZ(int x, int z)
    {
        return new Coords { X = x, Z = z, };
    }

    public static Coords FromOffset(int x, int z)
    {
        var coords = new Coords();

        var axial = Hex.Offset2Axial(new Vector3(x, 0, z));
        coords.X = (int)axial.X;
        coords.Z = (int)axial.Z;

        return coords;
    }

    public static Coords FromWorld(Vector3 position)
    {
        var coords = new Coords();

        var axial = Hex.World2Axial(position);
        coords.X = (int)axial.X;
        coords.Z = (int)axial.Z;

        return coords;
    }

    public static Coords FromCube(Vector3 cube)
    {
        var coords = new Coords();

        var axial = Hex.Cube2Axial(cube);
        coords.X = (int)axial.X;
        coords.Z = (int)axial.Z;

        return coords;
    }

    public Vector3 ToAxial()
    {
        return new Vector3(X, 0, Z);
    }

    public Vector3 ToOffset()
    {
        return Hex.Axial2Offset(ToAxial());
    }

    public Vector3 ToCube()
    {
        return Hex.Axial2Cube(ToAxial());
    }

    public Vector3 ToWorld()
    {
        return Hex.Offset2World(ToOffset());
    }

    public int DistanceTo(Coords otherCoords)
    {
        return Hex.GetDistance(ToCube(), otherCoords.ToCube());
    }

    public IEnumerable<(Direction, Coords)> GetNeighbors()
    {
        var i = 0;
        return Hex.GetNeighbors(ToCube()).Select(n => ((Direction)i++, FromCube(n)));
    }

    public bool IsNeighborOf(Coords otherCoords)
    {
        return DistanceTo(otherCoords) == 1;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Coords coords)
        {
            return coords.X == X && coords.Z == Z;
        }

        return false;
    }

    public override string ToString()
    {
        return $"({X}, {Z})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public static bool operator ==(Coords left, Coords right) => left.Equals(right);

    public static bool operator !=(Coords left, Coords right) => !left.Equals(right);
}