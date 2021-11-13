using Godot;

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

public struct MoveUnitMessage
{
    public Vector3 From { get; set; }
    public Vector3 To { get; set; }
}