using Godot;

public struct Coords
{
    int X;
    int Z;

    public Vector3 Axial
    {
        get { return new Vector3(X, 0, Z); }
    }

    public Vector3 Offset
    {
        get { return Hex.Axial2Offset(Axial); }
    }

    public Vector3 Cube
    {
        get { return Hex.Axial2Cube(Axial); }
    }

    public Vector3 World
    {
        get { return Hex.Offset2World(Offset); }
    }

    public bool IsNeighborOf(Coords otherCoords)
    {
        var neighbors = Hex.GetNeighbors(Cube);

        foreach (var nCube in neighbors)
        {
            if (nCube == otherCoords.Cube)
            {
                return true;
            }
        }

        return false;
    }

    public int DistanceTo(Coords otherCoords)
    {
        return Hex.GetDistance(Cube, otherCoords.Cube);
    }

    public static Coords FromCube(Vector3 cube)
    {
        var coords = new Coords();

        var axial = Hex.Cube2Axial(cube);
        coords.X = (int)axial.x;
        coords.Z = (int)axial.z;

        return coords;
    }

    public static Coords FromOffset(float x, float z)
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

    public int GetIndex(int width)
    {
        return (int)this.Offset.z * width + (int)this.Offset.x;
    }

    public override bool Equals(object obj)
    {
        if (obj is Coords coords)
        {
            return coords.X == this.X && coords.Z == this.Z;
        }

        return false;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Z);
    }
}