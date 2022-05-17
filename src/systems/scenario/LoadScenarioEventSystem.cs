using Godot;
using Bitron.Ecs;
using Nakama.TinyJson;

public struct LoadScenarioEvent
{
    public string Name { get; set; }

    public LoadScenarioEvent(string name)
    {
        Name = name;
    }
}

public class LoadScenarioEventSystem : IEcsSystem
{
    public static string Path = "user://saves/";

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<LoadScenarioEvent>().End();
        var startLocQuery = world.Query<IsStartingPositionOfSide>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var matchPlayers = world.GetResource<MatchPlayers>();

            var loadEvent = world.Entity(eventEntityId).Get<LoadScenarioEvent>();

            var saveData = Loader.LoadJson<ScenarioSaveData>(Path + loadEvent.Name + ".json");

            world.Spawn().Add(new SpawnScheduleEvent(saveData.Schedule, saveData.DaytimeIndex));
            world.Spawn().Add(new SpawnMapEvent(saveData.MapData));

            foreach (var sideData in saveData.Sides)
            {
                foreach (var startLocId in startLocQuery)
                {
                    var startLocEntity = world.Entity(startLocId);

                    if (startLocEntity.Get<IsStartingPositionOfSide>().Value != sideData.Side)
                    {
                        continue;
                    }

                    world.Spawn().Add(new SpawnPlayerEvent(sideData.Side, startLocEntity.Get<Coords>(), sideData.Faction, sideData.Gold));
                    break;
                }
            }

            foreach (var unitData in saveData.Units)
            {
                world.Spawn().Add(new SpawnUnitEvent(unitData));
            }
        }
    }
}
