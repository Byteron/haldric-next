using System;

public enum NetworkOperation
{
    PlayerReadied,
    FactionChanged,
    PlayerChanged,
    GoldChanged,
    TurnEnd,
    MoveUnit,
    RecruitUnit,
    AttackUnit,
}

public struct FactionChangedMessage
{
    public int Side { get; set; }
    public int Index { get; set; }
}

public struct PlayerChangedMessage
{
    public int Side { get; set; }
    public int Index { get; set; }
}

public struct GoldChangedMessage
{
    public int Side { get; set; }
    public int Value { get; set; }
}

public struct TurnEndMessage { }

public struct PlayerReadied { }

public struct MoveUnitMessage
{
    public Coords From { get; set; }
    public Coords To { get; set; }
}

public struct RecruitUnitMessage
{
    public int Side { get; set; }
    public Coords Coords { get; set; }
    public string UnitTypeId { get; set; }
}

public struct AttackUnitMessage
{
    public ulong Seed { get; set; }
    public int Distance { get; set; }
    public Coords From { get; set; }
    public Coords To { get; set; }
    public string AttackerAttackId { get; set; }
    public string DefenderAttackId { get; set; }
}