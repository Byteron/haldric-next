using System;
using Godot;

[Serializable]
public class Coords
{
    public static Coords FromOffset(int x, int z)
    {
        var coords = new Coords();

        var axial = Hex.Offset2Axial(new Vector3(x, 0, z));
        coords.X = (int)axial.x;
        coords.Z = (int)axial.z;

        return coords;
    }

    public static Coords FromWorld(Vector3 position)
    {
        var coords = new Coords();

        var axial = Hex.World2Axial(position);
        coords.X = (int)axial.x;
        coords.Z = (int)axial.z;

        return coords;
    }

    public static Coords FromCube(Vector3 cube)
    {
        var coords = new Coords();

        var axial = Hex.Cube2Axial(cube);
        coords.X = (int)axial.x;
        coords.Z = (int)axial.z;

        return coords;
    }

    public int X;
    public int Z;

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

    public bool IsNeighborOf(Coords otherCoords)
    {
        return DistanceTo(otherCoords) == 1;
    }

    public int GetIndex(int width)
    {
        return (int)this.ToOffset().z * width + (int)this.ToOffset().x;
    }

    public override bool Equals(object obj)
    {
        if (obj is Coords coords)
        {
            return coords.X == this.X && coords.Z == this.Z;
        }

        return false;
    }

    public Coords Clone()
    {
        return new  Coords { X = X, Z = Z };
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Z);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}