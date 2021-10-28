using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

struct RenderData
{
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
}

public partial class TerrainFeaturePopulator : Node3D
{
    private Dictionary<int, RID> _multiMeshRids = new Dictionary<int, RID>();
    private Dictionary<int, List<RenderData>> _renderData = new Dictionary<int, List<RenderData>>();
    private List<RID> _rids = new List<RID>();
    private Dictionary<Vector3, int> _randomIndicies = new Dictionary<Vector3, int>();
    public TerrainFeaturePopulator()
    {
        Name = "TerrainFeaturePopuplator";
    }

    public override void _ExitTree()
    {
        Clear();
    }

    public void Clear()
    {
        foreach (var rid in _rids)
        {
            RenderingServer.FreeRid(rid);
        }

        _multiMeshRids.Clear();
        _rids.Clear();
        _renderData.Clear();
    }

    public void Apply()
    {
        foreach (var item in _renderData)
        {
            var meshId = item.Key;
            var renderDatas = item.Value;

            RenderingServer.MultimeshAllocateData(_multiMeshRids[meshId], renderDatas.Count, RenderingServer.MultimeshTransformFormat.Transform3d);
            RenderingServer.MultimeshSetVisibleInstances(_multiMeshRids[meshId], renderDatas.Count);

            int index = 0;
            foreach (var renderData in renderDatas)
            {
                var xform = new Transform3D(Basis.Identity, renderData.Position);
                xform.basis = xform.basis.Rotated(Vector3.Up, renderData.Rotation.y);
                RenderingServer.MultimeshInstanceSetTransform(_multiMeshRids[meshId], index, xform);
                index += 1;
            }
        }
    }

    public void AddDecoration(EcsEntity locEntity, string terrainCode)
    {
        var position = locEntity.Get<Coords>().World;
        position.y = locEntity.Get<Elevation>().HeightWithOffset;

        foreach (var terrainGraphic in Data.Instance.Decorations[terrainCode].Values)
        {
            if (terrainGraphic.Variations.Count == 0)
            {
                AddRenderData(terrainGraphic.Mesh, position, Vector3.Zero);
            }
            else
            {
                if (!_randomIndicies.TryGetValue(position, out var index))
                {
                    index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                    _randomIndicies.Add(position, index);
                }
                
                var mesh = terrainGraphic.Variations[index];
                AddRenderData(mesh, position, Vector3.Zero);
            }
        }
    }

    public void AddDirectionalDecoration(EcsEntity locEntity, string terrainCode)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var plateauArea = ref locEntity.Get<PlateauArea>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        var center = locEntity.Get<Coords>().World;
        center.y = locEntity.Get<Elevation>().HeightWithOffset;

        var rotation = 240;
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            rotation += 60;

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;
            var nTerrainCode = nTerrainEntity.Get<TerrainCode>().Value;

            if (elevation.ValueWithOffset != nElevation.ValueWithOffset)
            {
                continue;
            }

            var position = center + Metrics.GetSolidEdgeMiddle(direction, plateauArea);

            foreach (var terrainGraphic in Data.Instance.DirectionalDecorations[terrainCode].Values)
            {
                if (terrainGraphic.Variations.Count == 0)
                {
                    AddRenderData(terrainGraphic.Mesh, position, new Vector3(0f, Mathf.Deg2Rad(rotation), 0f));
                }
                else
                {
                    if (!_randomIndicies.TryGetValue(position, out var index))
                    {
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        _randomIndicies.Add(position, index);
                    }
                    
                    var mesh = terrainGraphic.Variations[index];
                    AddRenderData(mesh, position, new Vector3(0f, Mathf.Deg2Rad(rotation), 0f));
                }
            }
            // AddRenderData(Data.Instance.DirectionalDecorations[terrainCode].Mesh, position, new Vector3(0f, Mathf.Deg2Rad(rotation), 0f));
        }
    }

    public void AddKeepPlateau(EcsEntity locEntity, string terrainCode)
    {
        var position = locEntity.Get<Coords>().World;

        position.y = locEntity.Get<Elevation>().HeightWithOffset;
        position += Data.Instance.KeepPlateaus[terrainCode].Offset;

        AddRenderData(Data.Instance.KeepPlateaus[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWater(EcsEntity locEntity, string terrainCode)
    {
        var position = locEntity.Get<Coords>().World;
        position.y = locEntity.Get<Elevation>().Height - Metrics.ElevationStep * 0.5f;

        AddRenderData(Data.Instance.WaterGraphics[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWalls(EcsEntity locEntity)
    {
        var coords = locEntity.Get<Coords>();
        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var terrainCode = terrainEntity.Get<TerrainCode>().Value;
        var elevation = locEntity.Get<Elevation>();
        var neighbors = locEntity.Get<Neighbors>();

        var center = locEntity.Get<Coords>().World;
        center.y = locEntity.Get<Elevation>().HeightWithOffset;

        var rotation = 240;
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            rotation += 60;

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;
            var nTerrainCode = nTerrainEntity.Get<TerrainCode>().Value;

            // if (elevation.Level < nElevation.Level)
            // {
            //     continue;
            // }

            if (elevation.ValueWithOffset == nElevation.ValueWithOffset && nTerrainEntity.Has<CanRecruitFrom>())
            {
                continue;
            }
            if (elevation.ValueWithOffset == nElevation.ValueWithOffset && !terrainEntity.Has<CanRecruitFrom>() && terrainEntity.Has<CanRecruitTo>() && nTerrainEntity.Has<CanRecruitTo>())
            {
                continue;
            }

            var wallPosition = center + Metrics.GetEdgeMiddle(direction);
            AddRenderData(Data.Instance.WallSegments[terrainCode].Mesh, wallPosition, new Vector3(0f, Mathf.Deg2Rad(rotation), 0f));
        }
    }

    public void AddTowers(EcsEntity locEntity)
    {
        var coords = locEntity.Get<Coords>();
        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var terrainCode = terrainEntity.Get<TerrainCode>().Value;
        var elevation = locEntity.Get<Elevation>();
        var plateauArea = locEntity.Get<PlateauArea>();
        var neighbors = locEntity.Get<Neighbors>();

        var center = locEntity.Get<Coords>().World;
        center.y = locEntity.Get<Elevation>().HeightWithOffset;

        var rotation = 240;
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            rotation += 60;

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;
            var nTerrainCode = nTerrainEntity.Get<TerrainCode>().Value;

            if (elevation.ValueWithOffset == nElevation.ValueWithOffset && nTerrainEntity.Has<CanRecruitFrom>())
            {
                continue;
            }
            if (elevation.ValueWithOffset == nElevation.ValueWithOffset && !terrainEntity.Has<CanRecruitFrom>() && terrainEntity.Has<CanRecruitTo>() && nTerrainEntity.Has<CanRecruitTo>())
            {
                continue;
            }

            var towerPosition = center + Metrics.GetFirstCorner(direction);
    
            AddRenderData(Data.Instance.WallTowers[terrainCode].Mesh, towerPosition, new Vector3(0f, Mathf.Deg2Rad(rotation), 0f));
        }
    }

    public void AddRenderData(Mesh mesh, Vector3 origin, Vector3 rotation)
    {
        var meshId = mesh.GetRid().GetId();

        if (!_multiMeshRids.ContainsKey(meshId))
        {
            _multiMeshRids.Add(meshId, NewMultiMesh(mesh));
        }

        if (!_renderData.ContainsKey(meshId))
        {
            _renderData.Add(meshId, new List<RenderData>());
        }

        var renderDatas = _renderData[meshId];

        var renderData = new RenderData
        {
            Position = origin,
            Rotation = rotation
        };
        renderDatas.Add(renderData);
    }

    private RID NewMultiMesh(Mesh mesh)
    {
        RID multimeshRID = RenderingServer.MultimeshCreate();
        RID instanceRID = RenderingServer.InstanceCreate();

        RID scenarioRID = GetWorld3d().Scenario;

        RenderingServer.MultimeshSetMesh(multimeshRID, mesh.GetRid());
        RenderingServer.InstanceSetScenario(instanceRID, scenarioRID);
        RenderingServer.InstanceSetBase(instanceRID, multimeshRID);

        _rids.Add(multimeshRID);
        _rids.Add(instanceRID);

        return multimeshRID;
    }
}
