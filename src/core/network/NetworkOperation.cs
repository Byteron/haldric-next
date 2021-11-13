using System;

public enum NetworkOperation
{
    FactionSelected,
    TurnEnd,
    MoveUnit,
    RecruitUnit,
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
    public Coords From { get; set; }
    public Coords To { get; set; }
}

[Serializable]
public struct RecruitUnitMessage
{
    public int Side { get; set; }
    public Coords Coords { get; set; }
    public string UnitTypeId { get; set; }
}