using System.Collections.Generic;
using Bitron.Ecs;

public class Scenario
{
    public int CurrentPlayer = -1;
    public List<EcsEntity> Players = new List<EcsEntity>();

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % Players.Count;
    }

    public EcsEntity GetCurrentPlayerEntity()
    {
        return Players[CurrentPlayer];
    }
}