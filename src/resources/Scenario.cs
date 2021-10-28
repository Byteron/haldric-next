using System.Collections.Generic;
using Bitron.Ecs;

public class Scenario
{
    public int Turn = 0;
    public int CurrentPlayer = -1;
    public List<EcsEntity> Players = new List<EcsEntity>();

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % Players.Count;

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