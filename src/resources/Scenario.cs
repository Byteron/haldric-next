using System.Collections.Generic;
using Bitron.Ecs;

public class Scenario
{
    public int Round { get; set; } = 0;
    public int Side { get; set; } = -1;
    public Dictionary<int, EcsEntity> Sides { get; set; } = new Dictionary<int, EcsEntity>();

    private int _round = -1;

    public void EndTurn()
    {
        Side = (Side + 1) % Sides.Count;

        if (Side == 0)
        {
            Round += 1;
        }
    }

    public EcsEntity GetCurrentSideEntity()
    {
        return Sides[Side];
    }

    public bool HasRoundChanged()
    {
        if (Round != _round)
        {
            _round = Round;
            return true;
        }

        return false;
    }
}