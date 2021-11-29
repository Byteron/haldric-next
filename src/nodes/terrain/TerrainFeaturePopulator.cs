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
        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

        var position = coords.World();
        position.y = elevation.Height + elevationOffset.Value;

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

                if (terrainGraphic.Variations.Count <= index)
                {
                    _randomIndicies.Remove(position);
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

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

        var center = coords.World();
        center.y = elevation.Height + elevationOffset.Value;

        for (int i = 0; i < 6; i++)
        {
            Direction direction = (Direction)i;

            float rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            ref var nElevation = ref nLocEntity.Get<Elevation>();
            ref var nTerrainBase = ref nLocEntity.Get<HasBaseTerrain>();

            var nTerrainEntity = nTerrainBase.Entity;
            ref var nTerrainCode = ref nTerrainEntity.Get<TerrainCode>();
            ref var nElevationOffset = ref nTerrainEntity.Get<ElevationOffset>();


            if (elevation.Value != nElevation.Value)
            {
                continue;
            }

            var elevationOffsetDifference = elevationOffset.Value - nElevationOffset.Value;

            if (Mathf.Abs(elevationOffsetDifference) > 0.5f)
            {
                continue;
            }

            var position = center + Metrics.GetSolidEdgeMiddle(direction, plateauArea);

            foreach (var terrainGraphic in Data.Instance.DirectionalDecorations[terrainCode].Values)
            {
                if (terrainGraphic.Variations.Count == 0)
                {
                    AddRenderData(terrainGraphic.Mesh, position, new Vector3(0f, rotation, 0f));
                }
                else
                {
                    if (!_randomIndicies.TryGetValue(position, out var index))
                    {
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        _randomIndicies.Add(position, index);
                    }

                    if (terrainGraphic.Variations.Count <= index)
                    {
                        _randomIndicies.Remove(position);
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        _randomIndicies.Add(position, index);
                    }

                    var mesh = terrainGraphic.Variations[index];
                    AddRenderData(mesh, position, new Vector3(0f, rotation, 0f));
                }
            }
            // AddRenderData(Data.Instance.DirectionalDecorations[terrainCode].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    public void AddKeepPlateau(EcsEntity locEntity, string terrainCode)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();
        var position = coords.World();

        position.y = elevation.Height + elevationOffset.Value;
        position += Data.Instance.KeepPlateaus[terrainCode].Offset;

        AddRenderData(Data.Instance.KeepPlateaus[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWater(EcsEntity locEntity, string terrainCode)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();
        var position = coords.World();
        position.y = elevation.Height - Metrics.ElevationStep * 0.5f;

        AddRenderData(Data.Instance.WaterGraphics[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWalls(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrainBase = ref locEntity.Get<HasBaseTerrain>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>();
        ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

        var center = coords.World();

        center.y = elevation.Height + elevationOffset.Value;
        for (int i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            //walls want to rotate the other way it seems?
            float rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            ref var nElevation = ref nLocEntity.Get<Elevation>();
            ref var nTerrainBase = ref nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;
            ref var nTerrainCode = ref nTerrainEntity.Get<TerrainCode>();

            if (nElevation.Value < 0)
            {
                continue;
            }

            if (elevation.Value == nElevation.Value && nTerrainEntity.Has<CanRecruitFrom>())
            {
                continue;
            }
            if (elevation.Value == nElevation.Value && !terrainEntity.Has<CanRecruitFrom>() && terrainEntity.Has<CanRecruitTo>() && nTerrainEntity.Has<CanRecruitTo>())
            {
                continue;
            }

            var wallPosition = center + Metrics.GetEdgeMiddle(direction);
            AddRenderData(Data.Instance.WallSegments[terrainCode.Value].Mesh, wallPosition, new Vector3(0f, rotation, 0f));
        }
    }

    public void AddCliffs(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrainBase = ref locEntity.Get<HasBaseTerrain>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>();
        ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

        var center = coords.World();

        center.y = elevation.Height + elevationOffset.Value;
        for (int i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            //walls want to rotate the other way it seems?
            float rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            ref var nElevation = ref nLocEntity.Get<Elevation>();
            ref var nTerrainBase = ref nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;
            ref var nTerrainCode = ref nTerrainEntity.Get<TerrainCode>();

            var elevationDiff = elevation.Value - nElevation.Value;
            
            if (elevationDiff < 2)
            {
                continue;
            }

            var cliffPosition = center + Metrics.GetEdgeMiddle(direction);
            AddRenderData(Data.Instance.Cliffs[terrainCode.Value].Mesh, cliffPosition, new Vector3(0f, rotation, 0f));
        }
    }

    public void AddTowers(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrainBase = ref locEntity.Get<HasBaseTerrain>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var plateauArea = ref locEntity.Get<PlateauArea>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>();
        ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

        var center = coords.World();
        center.y = elevation.Height + elevationOffset.Value;

        for (int i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            float rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            ref var nElevation = ref nLocEntity.Get<Elevation>();
            ref var nTerrainBase = ref nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;
            ref var nTerrainCode = ref nTerrainEntity.Get<TerrainCode>();

            if (nElevation.Value < 0)
            {
                continue;
            }

            if (elevation.Value == nElevation.Value && nTerrainEntity.Has<CanRecruitFrom>())
            {
                continue;
            }
            if (elevation.Value == nElevation.Value && !terrainEntity.Has<CanRecruitFrom>() && terrainEntity.Has<CanRecruitTo>() && nTerrainEntity.Has<CanRecruitTo>())
            {
                continue;
            }

            var towerPosition = center + Metrics.GetFirstCorner(direction);
            AddRenderData(Data.Instance.WallTowers[terrainCode.Value].Mesh, towerPosition, new Vector3(0f, rotation, 0f));
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
