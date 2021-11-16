using System.Collections.Generic;
using Bitron.Ecs;

public class Scenario
{
    public int Turn { get; set; } = 0;
    public int CurrentPlayer { get; set; } = -1;
    public EcsEntity[] Players { get; set; }

    public Scenario(int playerCount)
    {
        Players = new EcsEntity[playerCount];
    }

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % Players.Length;

        if (CurrentPlayer == 0)
        {
            Turn += 1;
        }
    }

    public EcsEntity GetCurrentPlayerEntity()
    {
        return Players[CurrentPlayer];
    }
}