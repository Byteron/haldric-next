using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public class SpawnMapSystem : IEcsInitSystem
{
    static int DefaultWidth = 40;
    static int DefaultHeight = 40;

    EcsWorld _world;
    Node _parent;

    public SpawnMapSystem(Node parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var dict = new Dictionary();

        dict.Add("Width", DefaultWidth);
        dict.Add("Height", DefaultHeight);

        var locsDict = new Dictionary();

        for (int z = 0; z < DefaultHeight; z++)
        {
            for (int x = 0; x < DefaultWidth; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var locDict = new Dictionary();

                locDict.Add("Terrain", new List<string>() { "Gg" });
                locDict.Add("Elevation", 0);

                if (locsDict.Contains(coords.Cube))
                {
                    locsDict[coords.Cube] = locDict;
                }
                else
                {
                    locsDict.Add(coords.Cube, locDict);
                }
            }
        }

        dict.Add("Locations", locsDict);

        dict = JSON.Parse(JSON.Print(dict)).Result as Dictionary;

        var eventEntity = _world.NewEntity();
        eventEntity.Replace(new CreateMapEvent(dict));
    }
}