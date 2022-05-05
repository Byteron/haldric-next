using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Godot;

public struct CheckVictoryConditionEvent { }

public class CheckVictoryConditionEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((CheckVictoryConditionEvent _e) =>
        {
            if (!commands.TryGetElement<Scenario>(out var scenario))
            {
                return;
            }

            var aliveFactions = new List<Entity>();

            foreach (var sideEntity in scenario.Sides.Values)
            {
                var leaderCount = 0;
                var playerSide = sideEntity.Get<Side>().Value;

                commands.ForEach((Entity unitEntity, ref Side unitSide, ref IsLeader isLeader) =>
                {
                    if (unitSide.Value == playerSide)
                    {
                        leaderCount += 1;
                    }
                });

                if (leaderCount > 0)
                {
                    aliveFactions.Add(sideEntity);
                }
            }

            if (aliveFactions.Count == 1)
            {
                var winningPlayer = aliveFactions[0];
                GD.Print($"Player {winningPlayer.Get<Side>().Value} won the game!");
                commands.GetElement<GameStateController>().PopState();
            }
        });
    }
}