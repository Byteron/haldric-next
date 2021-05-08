using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

struct RenderData
{
    public Vector3 Position;
    public Vector3 Rotation;
}

public partial class TerrainFeaturePopulator : Node3D
{
    private Dictionary<int, RID> _multiMeshInstances = new Dictionary<int, RID>();
    private Dictionary<int, List<RenderData>> _renderData = new Dictionary<int, List<RenderData>>();
    private Dictionary<int, RID> _rids = new Dictionary<int, RID>();

    public TerrainFeaturePopulator()
    {
        Name = "TerrainFeaturePopuplator";
    }

    public void Clear()
    {
        _renderData.Clear();
    }

    public void Apply()
    {
        foreach (var item in _renderData)
        {
            var multiMeshId = item.Key;
            var renderDatas = item.Value;

            RenderingServer.MultimeshAllocateData(_rids[multiMeshId], renderDatas.Count, RenderingServer.MultimeshTransformFormat.Transform3d);

            int index = 0;
            foreach (var renderData in renderDatas)
            {
                var xform = new Transform(Basis.Identity, renderData.Position);
                xform.basis = xform.basis.Rotated(Vector3.Up, renderData.Rotation.z);
                RenderingServer.MultimeshInstanceSetTransform(_rids[multiMeshId], index, xform);
                index += 1;
            }
        }
    }

    public void AddDecoration(EcsEntity locEntity)
    {
        ref var terrainEntity = ref locEntity.Get<HasTerrain>().Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>().Value;

        var position = locEntity.Get<Coords>().World;
        position.y = locEntity.Get<Elevation>().Height;

        AddRenderData(Data.Instance.Decorations[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddKeepPlateau(EcsEntity locEntity)
    {
        ref var terrainEntity = ref locEntity.Get<HasTerrain>().Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>().Value;

        var position = locEntity.Get<Coords>().World;
        position.y = locEntity.Get<Elevation>().Height + 1.5f;

        AddRenderData(Data.Instance.KeepPlateaus[terrainCode].Mesh, position, Vector3.Zero);
    }

    public void AddWalls(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrainEntity = ref locEntity.Get<HasTerrain>().Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>().Value;
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
            ref var nTerrainEntity = ref nLocEntity.Get<HasTerrain>().Entity;
            ref var nTerrainCode = ref nTerrainEntity.Get<TerrainCode>().Value;

            if (elevation.Level < nElevation.Level)
            {
                continue;
            }

            if (elevation.Level == nElevation.Level && Data.Instance.WallSegments.ContainsKey(nTerrainCode))
            {
                continue;
            }

            var wallPosition = center + Metrics.GetSolidEdgeMiddle(direction, plateauArea);

            AddRenderData(Data.Instance.WallSegments[terrainCode].Mesh, wallPosition, new Vector3(0f, 0f, Mathf.Deg2Rad(rotation)));
        }
    }

    public void AddTowers(EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var terrainEntity = ref locEntity.Get<HasTerrain>().Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>().Value;
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
            ref var nTerrainEntity = ref nLocEntity.Get<HasTerrain>().Entity;
            ref var nTerrainCode = ref nTerrainEntity.Get<TerrainCode>().Value;

            if (elevation.Level < nElevation.Level)
            {
                continue;
            }

            if (elevation.Level == nElevation.Level && Data.Instance.WallTowers.ContainsKey(nTerrainCode))
            {
                continue;
            }

            var towerPosition = center + Metrics.GetFirstCorner(direction);

            AddRenderData(Data.Instance.WallTowers[terrainCode].Mesh, towerPosition, new Vector3(0f, Mathf.Deg2Rad(rotation), 0f));
        }
    }

    public void AddRenderData(Mesh mesh, Vector3 origin, Vector3 rotation)
    {
        if (!_multiMeshInstances.ContainsKey(mesh.GetRid().GetId()))
        {
            _multiMeshInstances.Add(mesh.GetRid().GetId(), NewMultiMesh(mesh));
        }

        var multiMeshRid = _multiMeshInstances[mesh.GetRid().GetId()];

        if (!_rids.ContainsKey(multiMeshRid.GetId()))
        {
            _rids.Add(multiMeshRid.GetId(), multiMeshRid);
        }

        if (!_renderData.ContainsKey(multiMeshRid.GetId()))
        {
            _renderData.Add(multiMeshRid.GetId(), new List<RenderData>());
        }

        var renderDatas = _renderData[multiMeshRid.GetId()];

        var renderData = new RenderData();
        renderData.Position = origin;
        renderData.Rotation = rotation;
        renderDatas.Add(renderData);
    }

    private RID NewMultiMesh(Mesh mesh)
    {
        var multimesh = RenderingServer.MultimeshCreate();

        RID instance = RenderingServer.InstanceCreate();
        RID scenario = GetWorld3d().Scenario;

        RenderingServer.MultimeshSetMesh(multimesh, mesh.GetRid());
        RenderingServer.InstanceSetScenario(instance, scenario);
        RenderingServer.InstanceSetBase(instance, multimesh);

        return multimesh;
    }
}
