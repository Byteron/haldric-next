using Godot;
using System;
using Leopotam.Ecs;

public partial class TerrainFeaturePopulator : Node3D
{
    Node3D container = new Node3D();

    public TerrainFeaturePopulator()
    {
        Name = "TerrainFeaturePopuplator";
    }

    public override void _Ready()
    {
        Clear();
    }

    public void Clear()
    {
        container?.QueueFree();
        container = new Node3D();
        AddChild(container);
    }

    public void Apply()
    {
        // this currently does nothing, but should do something later on
    }

    public void AddDecoration(EcsEntity locEntity)
    {
        ref var terrain = ref locEntity.Get<Terrain>();

        var position = locEntity.Get<Coords>().World;
        position.y = locEntity.Get<Elevation>().Height;

        var forest = new MeshInstance3D();
        forest.Mesh = Data.Instance.Decorations[terrain.Code].Mesh;
        forest.Translation = position;
        container.AddChild(forest);
    }

    public void AddWalls(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrain = ref locEntity.Get<Terrain>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var plateauArea = ref locEntity.Get<PlateauArea>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        var center = locEntity.Get<Coords>().World;
        center.y = locEntity.Get<Elevation>().Height;

        var rotation = 240;
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            rotation += 60;

            if (!neighbors.Has(direction))
            {
                continue;
            }
            
            var nLocEntity = neighbors.Get(direction);
            
            ref var nElevation = ref nLocEntity.Get<Elevation>();
            
            if (elevation.Level < nElevation.Level)
            {
                continue;
            }

            if (elevation.Level == nElevation.Level)
            {
                continue;
            }
            
            var wallPosition = center + Metrics.GetSolidEdgeMiddle(direction, plateauArea);

            var wall = new MeshInstance3D();
            wall.Mesh = Data.Instance.WallSegments[terrain.Code].Mesh;
            wall.Translation = wallPosition;
            wall.RotationDegrees = new Vector3(0f, rotation, 0f);
            container.AddChild(wall);
        }
    }

    public void AddTowers(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrain = ref locEntity.Get<Terrain>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var plateauArea = ref locEntity.Get<PlateauArea>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        var center = locEntity.Get<Coords>().World;
        center.y = locEntity.Get<Elevation>().Height;

        var rotation = 240;
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            rotation += 60;

            if (!neighbors.Has(direction))
            {
                continue;
            }
            
            var nLocEntity = neighbors.Get(direction);
            
            ref var nElevation = ref nLocEntity.Get<Elevation>();
            
            if (elevation.Level < nElevation.Level)
            {
                continue;
            }

            if (elevation.Level == nElevation.Level)
            {
                continue;
            }

            var towerPosition = center + Metrics.GetFirstCorner(direction);

            var tower = new MeshInstance3D();
            tower.Mesh = Data.Instance.WallTowers[terrain.Code].Mesh;
            tower.Translation = towerPosition;
            tower.RotationDegrees = new Vector3(0f, rotation, 0f);
            container.AddChild(tower);
        }
    }
}
