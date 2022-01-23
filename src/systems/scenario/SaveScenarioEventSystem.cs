using Godot;
using Bitron.Ecs;
using Nakama.TinyJson;
using Haldric.Wdk;

public struct SaveScenarioEvent
{
    public string Name { get; set; }

    public SaveScenarioEvent(string name)
    {
        Name = name;
    }
}

public class SaveScenarioEventSystem : IEcsSystem
{
    public static string Path = "user://saves/";

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<SaveScenarioEvent>().End();
        var unitQuery = world.Query<Attribute<Health>>().End();
        var villageQuery = world.Query<Village>().Inc<IsCapturedBySide>().End();

        foreach (var eventEntityId in eventQuery)
        {
            ref var saveScenarioEvent = ref world.Entity(eventEntityId).Get<SaveScenarioEvent>();

            var scenario = world.GetResource<Scenario>();
            var schedule = world.GetResource<Schedule>();
            var map = world.GetResource<Map>();
            var locations = map.Locations;
            var grid = map.Grid;

            var saveGame = new ScenarioSaveData();

            saveGame.Round = scenario.Round;
            saveGame.Side = scenario.Side;
            saveGame.Schedule = schedule.Name;
            saveGame.DaytimeIndex = schedule.GetCurrentDaytimeIndex();
            
            foreach (var unitId in unitQuery)
            {
                var unitEntity = world.Entity(unitId);
                var unitData = UnitSaveData.FromUnitEntity(unitEntity);

                saveGame.Units.Add(unitData);
            }

            foreach (var entry in scenario.Sides)
            {
                var sideId = entry.Key;
                var sideEntity = entry.Value;

                var sideData = new SideSaveData();
                sideData.Side = sideEntity.Get<Side>().Value;
                sideData.Gold = sideEntity.Get<Gold>().Value;
                sideData.Faction = sideEntity.Get<Faction>().Value;
                
                foreach (var locId in villageQuery)
                {
                    var locEntity = world.Entity(locId);
                    var vSide = locEntity.Get<IsCapturedBySide>().Value;

                    if (sideData.Side == vSide)
                    {
                        sideData.Villages.Add(locEntity.Get<Coords>());
                    }
                }

                saveGame.Sides.Add(sideData);
            }

            var mapData = new MapData();

            mapData.Width = grid.Width;
            mapData.Height = grid.Height;

            foreach (var locEntity in locations.Dict.Values)
            {
                var coords = locEntity.Get<Coords>();

                var locData = MapDataLocation.FromLocEntity(locEntity);

                if (locEntity.Has<IsStartingPositionOfSide>())
                {
                    var playerMapData = new MapDataPlayer();
                    playerMapData.Coords = coords;
                    playerMapData.Side = locEntity.Get<IsStartingPositionOfSide>().Value;
                    mapData.Players.Add(playerMapData);
                }

                mapData.Locations.Add(locData);
            }

            saveGame.MapData = mapData;

            SaveToFile(saveScenarioEvent.Name, saveGame);
        }
    }

    private void SaveToFile(string name, ScenarioSaveData saveGame)
    {
        var dir = new Directory();
        dir.Open("user://");

        if (!dir.DirExists("saves"))
        {
            dir.MakeDir("saves");
        }

        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(saveGame.ToJson());
        file.Close();
    }
}