using Bitron.Ecs;

public class Scenario
{
    public int Round { get; set; } = 0;
    public int Side { get; set; } = -1;
    public EcsEntity[] Players { get; set; }

    private int _round = -1;

    public Scenario(int playerCount)
    {
        Players = new EcsEntity[playerCount];
    }

    public void EndTurn()
    {
        Side = (Side + 1) % Players.Length;

        if (Side == 0)
        {
            Round += 1;
        }
    }

    public EcsEntity GetCurrentPlayerEntity()
    {
        return Players[Side];
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