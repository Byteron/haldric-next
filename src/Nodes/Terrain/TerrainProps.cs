using System.Collections.Generic;
using Godot;

public struct RenderData
{
    public Vector3 Position;
    public Vector3 Rotation;
}

public partial class TerrainProps : Node3D
{
    public readonly Dictionary<ulong, Rid> MultiMeshRids = new();
    public readonly Dictionary<ulong, List<RenderData>> RenderData = new();
    public readonly List<Rid> Rids = new();
    public readonly Dictionary<Vector3, int> RandomIndices = new();
    
    public TerrainProps() => Name = "TerrainProps";

    public override void _ExitTree()
    {
        Clear();
    }

    public void Clear()
    {
        foreach (var rid in Rids)
        {
            RenderingServer.FreeRid(rid);
        }

        MultiMeshRids.Clear();
        Rids.Clear();
        RenderData.Clear();
    }

    public void Apply()
    {
        foreach (var (meshId, renderDataList) in RenderData)
        {
            RenderingServer.MultimeshAllocateData(MultiMeshRids[meshId], renderDataList.Count, RenderingServer.MultimeshTransformFormat.Transform3D);
            RenderingServer.MultimeshSetVisibleInstances(MultiMeshRids[meshId], renderDataList.Count);

            var index = 0;
            foreach (var renderData in renderDataList)
            {
                var xform = new Transform3D(Basis.Identity, renderData.Position);
                xform.Basis = xform.Basis.Rotated(Vector3.Up, renderData.Rotation.Y);
                RenderingServer.MultimeshInstanceSetTransform(MultiMeshRids[meshId], index, xform);
                index += 1;
            }
        }
    }

    public void AddRenderData(Resource mesh, Vector3 origin, Vector3 rotation)
    {
        var meshId = mesh.GetRid().Id;

        if (!MultiMeshRids.ContainsKey(meshId))
        {
            MultiMeshRids.Add(meshId, NewMultiMesh(mesh));
        }

        if (!RenderData.ContainsKey(meshId))
        {
            RenderData.Add(meshId, new List<RenderData>());
        }

        var renderDatas = RenderData[meshId];

        var renderData = new RenderData
        {
            Position = origin,
            Rotation = rotation
        };

        renderDatas.Add(renderData);
    }

    Rid NewMultiMesh(Resource mesh)
    {
        var multiMeshRid = RenderingServer.MultimeshCreate();
        var instanceRid = RenderingServer.InstanceCreate();

        var scenarioRid = GetWorld3D().Scenario;

        RenderingServer.MultimeshSetMesh(multiMeshRid, mesh.GetRid());
        RenderingServer.InstanceSetScenario(instanceRid, scenarioRid);
        RenderingServer.InstanceSetBase(instanceRid, multiMeshRid);

        Rids.Add(multiMeshRid);
        Rids.Add(instanceRid);

        return multiMeshRid;
    }
}