using System.Collections.Generic;
using RelEcs;

public class Scenario
{
    public int Round { get; private set; } = 0;
    public int Side { get; private set; } = -1;
    public Dictionary<int, Entity> Sides { get; } = new();

    int _round = -1;

    public void EndTurn()
    {
        Side = (Side + 1) % Sides.Count;

        if (Side == 0) Round += 1;
    }

    public Entity GetCurrentSideEntity()
    {
        return Sides[Side];
    }

    public bool HasRoundChanged()
    {
        if (Round == _round) return false;
        _round = Round;
        return true;
    }
}