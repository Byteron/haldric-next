using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public class SpawnMapSystem : IEcsInitSystem
{
    EcsWorld _world;
    Node _parent;

    public SpawnMapSystem(Node parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var dict = new Dictionary();

        dict.Add("Width", 20);
        dict.Add("Height", 20);

        var locsDict = new Dictionary();

        for (int z = 0; z < 20; z++)
        {
            for (int x = 0; x < 20; x++)
            {

                var coords = Coords.FromOffset(x, z);

                var locDict = new Dictionary();

                GD.Print(z, x, coords.Axial, coords.Cube);

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