using Godot;
using System;
using Leopotam.Ecs;

public struct SpawnUnitEvent
{
    public Coords Coords;

    public SpawnUnitEvent(Coords coords)
    {
        Coords = coords;
    }
}

public struct HasUnit
{
    public EcsEntity Entity;

    public HasUnit(EcsEntity entity)
    {
        Entity = entity;
    }
}

public class SpawnUnitEventSystem : IEcsRunSystem
{
    Node3D _parent;

    EcsWorld _world;
    EcsFilter<SpawnUnitEvent> _events;
    EcsFilter<Locations, Map> _maps;

    public SpawnUnitEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        if (_maps.IsEmpty())
        {
            return;
        }

        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            ref var spawnEvent = ref eventEntity.Get<SpawnUnitEvent>();

            var mapEntity = _maps.GetEntity(0);
            ref var locations = ref mapEntity.Get<Locations>();

            var locEntity = locations.Get(spawnEvent.Coords.Cube);
            ref var elevation = ref locEntity.Get<Elevation>();

            var unitEntity = _world.NewEntity();
            
            var unitView = Scenes.Instance.UnitView.Instance<UnitView>();
            
            _parent.AddChild(unitView);

            var translation = spawnEvent.Coords.World;
            translation.y = elevation.Height;

            unitView.Translation = translation;

            unitEntity.Replace(spawnEvent.Coords);
            unitEntity.Replace(new NodeHandle<UnitView>(unitView));

            locEntity.Replace(new HasUnit(unitEntity));

            eventEntity.Destroy();
        }
    }
}
