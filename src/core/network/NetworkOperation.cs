using System;
using Godot;

public struct SVector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public SVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

public static class Exentions
{
    public static Vector3 ToVector3(this SVector3 sv)
    {
        return new Vector3(sv.X, sv.Y, sv.Z);
    }

    public static SVector3 ToSVector3(this Vector3 v)
    {
        return new SVector3(v.x, v.y, v.z);
    }
}

public enum NetworkOperation
{
    FactionSelected,
    TurnEnd,
    MoveUnit,
}

public struct FactionSelectedMessage
{
    public int Side { get; set; }
    public int Index { get; set; }
}

public struct TurnEndMessage { }

[Serializable]
public struct MoveUnitMessage
{
    public SVector3 From { get; set; }
    public SVector3 To { get; set; }
}