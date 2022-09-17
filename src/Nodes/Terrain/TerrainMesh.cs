using System.Collections.Generic;
using Godot;

public partial class TerrainMesh : MeshInstance3D
{
    readonly List<int> _indices = new();
    readonly List<Vector3> _vertices = new();
    readonly List<int> _smoothGroups = new();
    readonly List<Vector3> _cellIndices = new();
    readonly List<Color> _cellWeights = new();

    SurfaceTool _surfaceTool;

    int _vertexIndex;

    public TerrainMesh() { Name = "TerrainMesh"; }

    public override void _Ready()
    {
        _surfaceTool = new SurfaceTool();
    }

    public void Clear()
    {
        _vertexIndex = 0;

        _indices.Clear();
        _vertices.Clear();
        _cellWeights.Clear();
        _cellIndices.Clear();

        _surfaceTool.Clear();

        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
    }

    public void Apply()
    {
        for (var i = 0; i < _vertices.Count; i++)
        {
            var type = _cellIndices[i];

            var xy = new Vector2(type.x, type.y);
            var xz = new Vector2(type.x, type.z);

            _surfaceTool.SetColor(_cellWeights[i]);
            _surfaceTool.SetSmoothGroup((uint)_smoothGroups[i]);
            _surfaceTool.SetUv(xy);
            _surfaceTool.SetUv2(xz);
            _surfaceTool.AddVertex(_vertices[i]);
        }

        for (var i = 0; i < _indices.Count; i++)
        {
            _surfaceTool.AddIndex(_indices[i]);
        }

        _surfaceTool.Index();

        _surfaceTool.GenerateNormals();

        Mesh = _surfaceTool.Commit();
    }

    public void AddTrianglePerturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        v1 = Metrics.Perturb(v1);
        v2 = Metrics.Perturb(v2);
        v3 = Metrics.Perturb(v3);

        AddTriangle(v1, v2, v3);
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);

        _smoothGroups.Add(_vertexIndex);
        _smoothGroups.Add(_vertexIndex);
        _smoothGroups.Add(_vertexIndex);

        _indices.Add(_vertexIndex);
        _indices.Add(_vertexIndex + 1);
        _indices.Add(_vertexIndex + 2);

        _vertexIndex += 3;
    }

    public void AddTriangleCellData(Vector3 indices, Color w) { AddTriangleCellData(indices, w, w, w); }

    public void AddTriangleCellData(Vector3 indices, Color w1, Color w2, Color w3)
    {
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);

        _cellWeights.Add(w1);
        _cellWeights.Add(w2);
        _cellWeights.Add(w3);
    }

    public void AddQuadPerturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        v1 = Metrics.Perturb(v1);
        v2 = Metrics.Perturb(v2);
        v3 = Metrics.Perturb(v3);
        v4 = Metrics.Perturb(v4);

        AddQuad(v1, v2, v3, v4);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _vertices.Add(v4);

        _smoothGroups.Add(_vertexIndex);
        _smoothGroups.Add(_vertexIndex);
        _smoothGroups.Add(_vertexIndex);
        _smoothGroups.Add(_vertexIndex);

        _indices.Add(_vertexIndex);
        _indices.Add(_vertexIndex + 1);
        _indices.Add(_vertexIndex + 2);
        _indices.Add(_vertexIndex + 2);
        _indices.Add(_vertexIndex + 1);
        _indices.Add(_vertexIndex + 3);

        _vertexIndex += 4;
    }

    public void AddQuadCellData(Vector3 indices, Color w) { AddQuadCellData(indices, w, w, w, w); }

    public void AddQuadCellData(Vector3 indices, Color w1, Color w2) { AddQuadCellData(indices, w1, w1, w2, w2); }

    public void AddQuadCellData(Vector3 indices, Color w1, Color w2, Color w3, Color w4)
    {
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);

        _cellWeights.Add(w1);
        _cellWeights.Add(w2);
        _cellWeights.Add(w3);
        _cellWeights.Add(w4);
    }
}