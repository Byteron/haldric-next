using Godot;
using System.Collections.Generic;

public partial class TerrainMesh : MeshInstance3D
{

	[Export] private bool  _useMesh, _useCollider;

	private CollisionShape3D _collisionShape;

	private SurfaceTool _surfaceTool;

	private int _vertexIndex = 0;

	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape3D>("StaticBody3D/CollisionShape3D");
		_surfaceTool = new SurfaceTool();
	}

	public void Clear()
	{
		_vertexIndex = 0;
		_surfaceTool.Clear();
		_surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
	}

	public void Apply()
	{
		_surfaceTool.Index();
		var material = new StandardMaterial3D();
		material.AlbedoColor = new Color("70483c");
		_surfaceTool.SetMaterial(material);
		_surfaceTool.GenerateNormals();

		Mesh = _surfaceTool.Commit();

		if (_useCollider)
		{
			_collisionShape.Shape = Mesh.CreateTrimeshShape();
		}

		if (!_useMesh)
		{
			Mesh = null;
		}
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
		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v1);

		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v2);
		
		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v3);

		_surfaceTool.AddIndex(_vertexIndex);
		_surfaceTool.AddIndex(_vertexIndex + 1);
		_surfaceTool.AddIndex(_vertexIndex + 2);

		_vertexIndex += 3;
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
		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v1);

		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v2);
		
		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v3);

		_surfaceTool.SetSmoothGroup((uint)_vertexIndex);
		_surfaceTool.AddVertex(v4);

		_surfaceTool.AddIndex(_vertexIndex);
		_surfaceTool.AddIndex(_vertexIndex + 1);
		_surfaceTool.AddIndex(_vertexIndex + 2);
		_surfaceTool.AddIndex(_vertexIndex + 2);
		_surfaceTool.AddIndex(_vertexIndex + 1);
		_surfaceTool.AddIndex(_vertexIndex + 3);

		_vertexIndex += 4;
	}
}
