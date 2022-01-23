using System.Collections.Generic;
using Bitron.Ecs;
using Godot;

public struct CheckVictoryConditionEvent { }

public class CheckVictoryConditionEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((ref CheckVictoryConditionEvent _e) =>
        {
            if (!world.TryGetResource<Scenario>(out var scenario))
            {
                return;
            }

            var aliveFactions = new List<EcsEntity>();

            foreach (var sideEntity in scenario.Sides.Values)
            {
                var leaderCount = 0;
                var playerSide = sideEntity.Get<Side>().Value;

                world.ForEach((EcsEntity unitEntity, ref Side unitSide, ref IsLeader isLeader) =>
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
                world.GetResource<GameStateController>().PopState();
            }
        });
    }
}