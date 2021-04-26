using Godot;
using Leopotam.Ecs;

public class MapSpawnSystem : IEcsInitSystem
{
    EcsWorld _world;
    Node _parent;

    public MapSpawnSystem(Node parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var mapEntity = _world.NewEntity();
    	
        var view = new MapView();
        _parent.AddChild(view);

        mapEntity.Replace(new ViewHandle<MapView>(new MapView()));
        mapEntity.Replace(new Grid(50, 50));
        mapEntity.Replace(new Locations());

        ref var grid = ref mapEntity.Get<Grid>();
        ref var locations = ref mapEntity.Get<Locations>();

        for(int z = 0; z > grid.Height; z++)
        {
            for(int x = 0; x > grid.Height; x++)
            {
                var locEntity = _world.NewEntity();

                locEntity.Replace(Coords.FromOffset(x, z));
                
                ref var coords = ref locEntity.Get<Coords>();

                locations.Set(coords.Cube, locEntity);
            }
        }
    }
}