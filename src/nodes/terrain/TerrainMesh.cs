using System.Collections.Generic;
using Godot;

public partial class TerrainMesh : MeshInstance3D
{
	private List<int> _indices = new List<int>();
	private List<Vector3> _vertices = new List<Vector3>();
	private List<uint> _smoothGroups = new List<uint>();
	private List<Color> _colors = new List<Color>();
	private List<Vector2> _uvs = new List<Vector2>();
	private List<Vector2> _uv2s = new List<Vector2>();
	private List<Vector3> _terrainTypes = new List<Vector3>();

	private SurfaceTool _surfaceTool;

	private int _vertexIndex = 0;
	
	public TerrainMesh()
	{
		Name = "TerrainMesh";
	}

	public override void _Ready()
	{
		_surfaceTool = new SurfaceTool();

		var material = GD.Load<Material>("res://assets/graphics/materials/terrain_material.tres");
		material.Set("shader_param/textures", Data.Instance.TextureArray);
		MaterialOverride = material;
	}

	public void Clear()
	{
		_vertexIndex = 0;
		
		_indices.Clear();
		_vertices.Clear();
		_colors.Clear();
		_uvs.Clear();
		_uv2s.Clear();
		_terrainTypes.Clear();

		_surfaceTool.Clear();
		
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
	}

	public void Apply()
	{
		for (int i = 0; i < _vertices.Count; i++)
		{
			var type = _terrainTypes[i];

			var xy = new Vector2(type.x, type.y);
			var xz = new Vector2(type.x, type.z);

			_surfaceTool.SetColor(_colors[i]);
			_surfaceTool.SetSmoothGroup(_smoothGroups[i]);
			_surfaceTool.SetUv(xy);
			_surfaceTool.SetUv2(xz);
			_surfaceTool.AddVertex(_vertices[i]);
		}

		for (int i = 0; i < _indices.Count; i++)
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
		
		_smoothGroups.Add((uint)_vertexIndex);
		_smoothGroups.Add((uint)_vertexIndex);
		_smoothGroups.Add((uint)_vertexIndex);

		_indices.Add(_vertexIndex);
		_indices.Add(_vertexIndex + 1);
		_indices.Add(_vertexIndex + 2);

		_vertexIndex += 3;
	}

	public void AddTriangleColor(Color c)
	{
		AddTriangleColor(c, c, c);
	}

	public void AddTriangleColor(Color c1, Color c2, Color c3)
	{
		_colors.Add(c1);
		_colors.Add(c2);
		_colors.Add(c3);
	}

	public void AddTriangleTerrainTypes(Vector3 types)
	{
		_terrainTypes.Add(types);
		_terrainTypes.Add(types);
		_terrainTypes.Add(types);
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

		_smoothGroups.Add((uint)_vertexIndex);
		_smoothGroups.Add((uint)_vertexIndex);
		_smoothGroups.Add((uint)_vertexIndex);
		_smoothGroups.Add((uint)_vertexIndex);

		_indices.Add(_vertexIndex);
		_indices.Add(_vertexIndex + 1);
		_indices.Add(_vertexIndex + 2);
		_indices.Add(_vertexIndex + 2);
		_indices.Add(_vertexIndex + 1);
		_indices.Add(_vertexIndex + 3);

		_vertexIndex += 4;
	}

	public void AddQuadColor(Color c1, Color c2)
	{
		AddQuadColor(c1, c1, c2, c2);
	}

	public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
	{
		_colors.Add(c1);
		_colors.Add(c2);
		_colors.Add(c3);
		_colors.Add(c4);
	}

	public void AddQuadTerrainTypes(Vector3 types)
	{
		_terrainTypes.Add(types);
		_terrainTypes.Add(types);
		_terrainTypes.Add(types);
		_terrainTypes.Add(types);
	}
}
