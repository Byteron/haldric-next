using System.Collections.Generic;
using Bitron.Ecs;

public class Scenario
{
    public int CurrentPlayer { get; set; } = -1;
    public List<EcsEntity> Players { get; set; } = new List<EcsEntity>();

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % Players.Count;
    }

    public EcsEntity GetCurrentPlayerEntity()
    {
        return Players[CurrentPlayer];
    }
}