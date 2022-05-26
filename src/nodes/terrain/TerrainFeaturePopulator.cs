using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

struct RenderData
{
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
}

public partial class TerrainFeaturePopulator : Node3D
{
     Dictionary<int, RID> _multiMeshRids = new();
     Dictionary<int, List<RenderData>> _renderData = new();
     List<RID> _rids = new();
     Dictionary<Vector3, int> _randomIndicies = new();
    public TerrainFeaturePopulator() => Name = "TerrainFeaturePopuplator";

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
        foreach (var (meshId, renderDatas) in _renderData)
        {
            RenderingServer.MultimeshAllocateData(_multiMeshRids[meshId], renderDatas.Count, RenderingServer.MultimeshTransformFormat.Transform3d);
            RenderingServer.MultimeshSetVisibleInstances(_multiMeshRids[meshId], renderDatas.Count);

            var index = 0;
            foreach (var renderData in renderDatas)
            {
                var xform = new Transform3D(Basis.Identity, renderData.Position);
                xform.basis = xform.basis.Rotated(Vector3.Up, renderData.Rotation.y);
                RenderingServer.MultimeshInstanceSetTransform(_multiMeshRids[meshId], index, xform);
                index += 1;
            }
        }
    }

    public void AddDecoration(Entity locEntity, string terrainCode)
    {
        var coords = locEntity.Get<Coords>();
        var elevation = locEntity.Get<Elevation>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

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

    public void AddDirectionalDecoration(Entity locEntity, string terrainCode)
    {
        var coords = locEntity.Get<Coords>();
        var elevation = locEntity.Get<Elevation>();
        var plateauArea = locEntity.Get<PlateauArea>();
        var neighbors = locEntity.Get<Neighbors>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        var center = coords.World();
        center.y = elevation.Height + elevationOffset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            var rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainBase = nLocEntity.Get<HasBaseTerrain>();

            var nTerrainEntity = nTerrainBase.Entity;
            var nElevationOffset = nTerrainEntity.Get<ElevationOffset>();


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

    public void AddKeepPlateau(Entity locEntity, string terrainCode)
    {
        var coords = locEntity.Get<Coords>();
        var elevation = locEntity.Get<Elevation>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var elevationOffset = terrainEntity.Get<ElevationOffset>();
        var position = coords.World();

        position.y = elevation.Height + elevationOffset.Value;
        position += Data.Instance.KeepPlateaus[terrainCode].Offset;

        AddRenderData(Data.Instance.KeepPlateaus[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWater(Entity locEntity, string terrainCode)
    {
        var coords = locEntity.Get<Coords>();
        var elevation = locEntity.Get<Elevation>();
        var position = coords.World();
        position.y = elevation.Height - Metrics.ElevationStep * 0.5f;

        AddRenderData(Data.Instance.WaterGraphics[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWalls(Entity locEntity)
    {
        var coords = locEntity.Get<Coords>();
        var terrainBase = locEntity.Get<HasBaseTerrain>();
        var elevation = locEntity.Get<Elevation>();
        var neighbors = locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        var terrainCode = terrainEntity.Get<TerrainCode>();
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        var center = coords.World();

        center.y = elevation.Height + elevationOffset.Value;
        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            //walls want to rotate the other way it seems?
            var rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainBase = nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;

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

            var position = center + Metrics.GetEdgeMiddle(direction);
            AddRenderData(Data.Instance.WallSegments[terrainCode.Value].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    public void AddOuterCliffs(Entity locEntity)
    {
        var coords = locEntity.Get<Coords>();
        var terrainBase = locEntity.Get<HasBaseTerrain>();
        var elevation = locEntity.Get<Elevation>();
        var neighbors = locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        var terrainCode = terrainEntity.Get<TerrainCode>();
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        var center = coords.World();
        center.y = elevation.Height + elevationOffset.Value;

        for (int i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            // cliffs want to rotate the other way it seems?
            var rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainBase = nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;
            var nTerrainCode = nTerrainEntity.Get<TerrainCode>();

            if (Data.Instance.InnerCliffs.ContainsKey(nTerrainCode.Value))
            {
                continue;
            }

            var elevationDiff = elevation.Value - nElevation.Value;
            
            if (elevationDiff < 2)
            {
                continue;
            }

            var position = center + Metrics.GetEdgeMiddle(direction);

            foreach (var terrainGraphic in Data.Instance.OuterCliffs[terrainCode.Value].Values)
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
        }
    }

    public void AddInnerCliffs(Entity locEntity)
    {
        var coords = locEntity.Get<Coords>();
        var terrainBase = locEntity.Get<HasBaseTerrain>();
        var elevation = locEntity.Get<Elevation>();
        var neighbors = locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        var terrainCode = terrainEntity.Get<TerrainCode>();

        var center = coords.World();

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            // cliffs want to rotate the other way it seems?
            var rotation = direction.Opposite().Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);
            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainBase = nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;
            var nElevationOffset = nTerrainEntity.Get<ElevationOffset>();

            var elevationDiff = nElevation.Value - elevation.Value;
            
            if (elevationDiff < 2)
            {
                continue;
            }

            center.y = nElevation.Height + nElevationOffset.Value;
            
            var position = center + Metrics.GetEdgeMiddle(direction);
            
            foreach (var terrainGraphic in Data.Instance.InnerCliffs[terrainCode.Value].Values)
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
        }
    }

    public void AddTowers(Entity locEntity)
    {
        var coords = locEntity.Get<Coords>();
        var terrainBase = locEntity.Get<HasBaseTerrain>();
        var elevation = locEntity.Get<Elevation>();
        var neighbors = locEntity.Get<Neighbors>();

        var terrainEntity = terrainBase.Entity;
        var terrainCode = terrainEntity.Get<TerrainCode>();
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        var center = coords.World();
        center.y = elevation.Height + elevationOffset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            var rotation = direction.Rotation();

            if (!neighbors.Has(direction))
            {
                continue;
            }

            var nLocEntity = neighbors.Get(direction);

            var nElevation = nLocEntity.Get<Elevation>();
            var nTerrainBase = nLocEntity.Get<HasBaseTerrain>();
            var nTerrainEntity = nTerrainBase.Entity;

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

            var position = center + Metrics.GetFirstCorner(direction);
            AddRenderData(Data.Instance.WallTowers[terrainCode.Value].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    void AddRenderData(Mesh mesh, Vector3 origin, Vector3 rotation)
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

     RID NewMultiMesh(Mesh mesh)
    {
        var multimeshRID = RenderingServer.MultimeshCreate();
        var instanceRID = RenderingServer.InstanceCreate();

        var scenarioRID = GetWorld3d().Scenario;

        RenderingServer.MultimeshSetMesh(multimeshRID, mesh.GetRid());
        RenderingServer.InstanceSetScenario(instanceRID, scenarioRID);
        RenderingServer.InstanceSetBase(instanceRID, multimeshRID);

        _rids.Add(multimeshRID);
        _rids.Add(instanceRID);

        return multimeshRID;
    }
}
