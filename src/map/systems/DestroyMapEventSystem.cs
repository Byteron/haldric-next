using Godot;
using Leopotam.Ecs;

public struct DestroyMapEvent { }

public class DestroyMapEventSystem : IEcsRunSystem
{
    EcsFilter<DestroyMapEvent> _events;
    EcsFilter<Map> _maps;
    EcsFilter<Chunk> _chunks;
    EcsFilter<MapCursor> _mapCursors;
    EcsFilter<NodeHandle<UnitView>> _units;

    public void Run()
    {
        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            var destroyEvent = eventEntity.Get<DestroyMapEvent>();

            foreach (var j in _chunks)
            {
                var chunkEntity = _chunks.GetEntity(j);

                ref var terrainMesh = ref chunkEntity.Get<NodeHandle<TerrainMesh>>().Node;
                ref var terrainCollider = ref chunkEntity.Get<NodeHandle<TerrainCollider>>().Node;
                ref var terrainFeaturePopulator = ref chunkEntity.Get<NodeHandle<TerrainFeaturePopulator>>().Node;

                terrainMesh.QueueFree();
                terrainCollider.QueueFree();
                terrainFeaturePopulator.QueueFree();

                chunkEntity.Destroy();
            }

            foreach (var j in _units)
            {
                var unitEntity = _units.GetEntity(j);

                ref var view = ref unitEntity.Get<NodeHandle<UnitView>>().Node;

                view.QueueFree();

                unitEntity.Destroy();
            }

            foreach (var j in _mapCursors)
            {
                var cursorEntity = _mapCursors.GetEntity(j);

                ref var highlighter = ref cursorEntity.Get<NodeHandle<Node3D>>().Node;

                highlighter.QueueFree();

                cursorEntity.Destroy();
            }

            foreach (var j in _maps)
            {
                var mapEntity = _maps.GetEntity(j);

                ref var locations = ref mapEntity.Get<Locations>();

                foreach (var locEntity in locations.Values)
                {
                    locEntity.Destroy();
                }
                
                mapEntity.Destroy();
            }

            eventEntity.Destroy();
        }
    }
}