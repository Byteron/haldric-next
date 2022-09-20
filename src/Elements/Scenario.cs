using System.Collections.Generic;
using RelEcs;

public class Scenario
{
    public int Round;
    public int Side = -1;
    public Dictionary<int, Entity> Sides = new();

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