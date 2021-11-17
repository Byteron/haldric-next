using Bitron.Ecs;

public class Scenario
{
    public int Round { get; set; } = 0;
    public int CurrentPlayer { get; set; } = -1;
    public EcsEntity[] Players { get; set; }

    private int _round = -1;

    public Scenario(int playerCount)
    {
        Players = new EcsEntity[playerCount];
    }

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % Players.Length;

        if (CurrentPlayer == 0)
        {
            Round += 1;
        }
    }

    public EcsEntity GetCurrentPlayerEntity()
    {
        return Players[CurrentPlayer];
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