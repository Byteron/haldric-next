using Bitron.Ecs;

public struct Team
{
    public int Value;

    public Team(int value)
    {
        Value = value;
    }
}

public class Scenario
{
    public int PlayerCount;
    public int CurrentPlayer;

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;
    }
}
public struct TurnEndEvent { }

public class TurnEndEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<TurnEndEvent>().End();
        var unitQuery = world.Query<Team>().Inc<Attribute<Moves>>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();
            scenario.EndTurn();
            
            Godot.GD.Print("Player: " + scenario.CurrentPlayer);
            Godot.GD.Print("Units: ", unitQuery.GetEntitiesCount());

            foreach (var unitEntityId in unitQuery)
            {
                var team = unitQuery.Get<Team>(unitEntityId);
                if (team.Value == scenario.CurrentPlayer)
                {
                    ref var moves = ref unitQuery.Get<Attribute<Moves>>(unitEntityId);
                    moves.Restore();
                }
            }
        }
    }
}