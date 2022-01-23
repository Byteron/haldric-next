using System.Collections.Generic;
using Bitron.Ecs;

public class SideSaveData
{
    public int Side = 0;
    public int Gold = 0;
    public string Faction = "";
    public List<Coords> Villages = new List<Coords>();
}

public class UnitSaveData
{
    public Coords Coords = default;
    public int Side = 0;
    public string UnitTypeId = "";
    public int Health = 0;
    public int Actions = 0;
    public int Moves = 0;
    public int Experince = 0;
    public bool IsHero = false;
    public bool IsLeader = false;

    public static UnitSaveData FromUnitEntity(EcsEntity unitEntity)
    {
        var data = new UnitSaveData();
        
        data.Coords = unitEntity.Get<Coords>();

        data.Side = unitEntity.Get<Side>().Value;
        data.UnitTypeId = unitEntity.Get<Id>().Value;
        
        data.Health = unitEntity.Get<Attribute<Health>>().Value;
        data.Actions = unitEntity.Get<Attribute<Actions>>().Value;
        data.Moves = unitEntity.Get<Attribute<Moves>>().Value;
        data.Experince = unitEntity.Get<Attribute<Experience>>().Value;

        if (unitEntity.Has<IsLeader>())
        {
            data.IsLeader = true;
        }

        if (unitEntity.Has<IsHero>())
        {
            data.IsHero = true;
        }

        return data;
    }
}

public class ScenarioSaveData
{
    public int Version = 1;
    public int Round = 1;
    public int Side = 0;
    public string Schedule = "";
    public int DaytimeIndex = 0;
    public List<SideSaveData> Sides = new List<SideSaveData>();
    public List<UnitSaveData> Units = new List<UnitSaveData>();
    public MapData MapData = null;
}