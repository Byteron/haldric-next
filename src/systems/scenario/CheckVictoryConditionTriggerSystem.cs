using System.Collections.Generic;
using System.IO.IsolatedStorage;
using RelEcs;
using RelEcs.Godot;
using Godot;

public class CheckVictoryConditionTrigger { }

public class CheckVictoryConditionTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((CheckVictoryConditionTrigger _) =>
        {
            if (!commands.TryGetElement<Scenario>(out var scenario)) return;

            var aliveFactions = new List<Entity>();

            foreach (var sideEntity in scenario.Sides.Values)
            {
                var leaderCount = 0;
                var playerSide = sideEntity.Get<Side>().Value;

                var query = commands.Query<Side>().Has<IsLeader>();

                foreach (var side in query)
                {
                    if (side.Value == playerSide)
                    {
                        leaderCount += 1;
                    }
                }

                if (leaderCount > 0)
                {
                    aliveFactions.Add(sideEntity);
                }
            }

            if (aliveFactions.Count != 1) return;
            
            var winningPlayer = aliveFactions[0];
            GD.Print($"Player {winningPlayer.Get<Side>().Value} won the game!");
            commands.GetElement<GameStateController>().PopState();
        });
    }
}