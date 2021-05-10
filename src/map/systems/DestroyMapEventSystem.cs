using Godot;
using Leopotam.Ecs;

public struct DestroyMapEvent { }

public class DestroyMapEventSystem : IEcsRunSystem
{
    EcsFilter<DestroyMapEvent> _events;
    EcsFilter<Map> _maps;
    EcsFilter<Chunk> _chunks;
    EcsFilter<Highlighter> _highlighter;
    EcsFilter<HoveredCoords> _hoveredCoords;

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

            foreach (var j in _highlighter)
            {
                var highlighterEntity = _highlighter.GetEntity(j);

                ref var highlighter = ref highlighterEntity.Get<NodeHandle<Node3D>>().Node;

                highlighter.QueueFree();

                highlighterEntity.Destroy();
            }

            foreach (var j in _hoveredCoords)
            {
                var mapEntity = _hoveredCoords.GetEntity(j);
                mapEntity.Destroy();
            }

            foreach (var j in _maps)
            {
                var mapEntity = _maps.GetEntity(j);
                mapEntity.Destroy();
            }

            eventEntity.Destroy();
        }
    }
}