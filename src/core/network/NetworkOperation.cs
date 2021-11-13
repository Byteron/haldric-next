using System;

public enum NetworkOperation
{
    FactionSelected,
    TurnEnd,
    MoveUnit,
    RecruitUnit,
    AttackUnit,
}

public struct FactionSelectedMessage
{
    public int Side { get; set; }
    public int Index { get; set; }
}

public struct TurnEndMessage { }

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