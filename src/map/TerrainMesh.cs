using Godot;
using System.Collections.Generic;

public struct EdgeVertices
{
	public Vector3 v1, v2, v3, v4, v5;

	public EdgeVertices(Vector3 corner1, Vector3 corner2)
	{
		v1 = corner1;
		v2 = corner1.Lerp(corner2, 0.25f);
		v3 = corner1.Lerp(corner2, 0.5f);
		v4 = corner1.Lerp(corner2, 0.75f);
		v5 = corner2;
	}

	public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep)
	{
		v1 = corner1;
		v2 = corner1.Lerp(corner2, outerStep);
		v3 = corner1.Lerp(corner2, 0.5f);
		v4 = corner1.Lerp(corner2, 1f - outerStep);
		v5 = corner2;
	}
}

public partial class TerrainMesh : MeshInstance3D
{

	private List<Vector3> vertices, normals, cellIndices;
	private List<Vector2> uvs = new List<Vector2>();
	private List<Vector2> uv2s = new List<Vector2>();
	private List<Color> cellWeights;

	private List<int> triangles;


	[Export] private bool useCollder, useMesh, useCellData, useUVCoordinates, useUV2Coordinates;

	private CollisionShape3D collisionShape;

	public override void _Ready()
	{
		collisionShape = GetNode<CollisionShape3D>("StaticBody3D/CollisionShape3D");
	}

	public void Clear()
	{
		vertices = ListPool<Vector3>.Get();
		triangles = ListPool<int>.Get();
		normals = ListPool<Vector3>.Get();

		if (useCellData)
		{
			cellWeights = ListPool<Color>.Get();
			cellIndices = ListPool<Vector3>.Get();
		}

		if (useUVCoordinates)
		{
			uvs = ListPool<Vector2>.Get();
		}

		if (useUV2Coordinates)
		{
			uv2s = ListPool<Vector2>.Get();
		}
	}

	public void Apply()
	{
		var mesh = new ArrayMesh();
		var arrays = new Godot.Collections.Array();

		arrays.Resize((int)ArrayMesh.ArrayType.Max);

		bool drawMesh = false;

		// GD.Print(Name);

		if (vertices.Count > 0)
		{
			//GD.Print("vertices: ", vertices.Count);
			//GD.Print("Normals: ", normals.Count);
			//GD.Print("triangles: ", triangles.Count);
			arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices.ToArray();
			arrays[(int)ArrayMesh.ArrayType.Normal] = normals.ToArray();
			// arrays[(int)ArrayMesh.ArrayType.Index] = triangles.ToArray();
			ListPool<Vector3>.Add(vertices);
			ListPool<Vector3>.Add(normals);
			ListPool<int>.Add(triangles);
			drawMesh = true;
		}

		if (useCellData && cellWeights.Count > 0)
		{
			// ("Weights: ", cellWeights.Count);
			// GD.Print("Indices: ", cellIndices.Count);
			arrays[(int)ArrayMesh.ArrayType.Color] = cellWeights.ToArray();

			Vector2[] xy = new Vector2[cellIndices.Count];
			Vector2[] xz = new Vector2[cellIndices.Count];

			for (int i = 0; i < cellIndices.Count; i++)
			{
				xy[i] = new Vector2(cellIndices[i].x, cellIndices[i].y);
				xz[i] = new Vector2(cellIndices[i].x, cellIndices[i].z);
			}

			arrays[(int)ArrayMesh.ArrayType.TexUv] = xy;
			arrays[(int)ArrayMesh.ArrayType.TexUv2] = xz;

			ListPool<Color>.Add(cellWeights);
			ListPool<Vector3>.Add(cellIndices);

			drawMesh = true;
		}

		if (useUVCoordinates && uvs.Count > 0)
		{
			// GD.Print("UVs: ", uvs.Count);
			arrays[(int)ArrayMesh.ArrayType.TexUv] = uvs.ToArray();
			ListPool<Vector2>.Add(uvs);
			drawMesh = true;
		}

		if (useUV2Coordinates && uv2s.Count > 0)
		{
			// GD.Print("UV2s: ", uv2s.Count);
			arrays[(int)ArrayMesh.ArrayType.TexUv2] = uv2s.ToArray();
			ListPool<Vector2>.Add(uv2s);
			drawMesh = true;
		}

		if (drawMesh)
		{
			mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		}

		Mesh = mesh;

		if (useCollder)
		{
			collisionShape.Shape = Mesh.CreateTrimeshShape();
		}

		if (!useMesh)
		{
			Mesh = null;
		}

	}

	public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;

		v1 = Metrics.Perturb(v1);
		v2 = Metrics.Perturb(v2);
		v3 = Metrics.Perturb(v3);

		vertices.Add(v1);
		vertices.Add(v3);
		vertices.Add(v2);

		normals.Add(CalculateNormal(v1, v3, v2));
		normals.Add(CalculateNormal(v3, v2, v1));
		normals.Add(CalculateNormal(v2, v1, v3));

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
	}

	public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;

		vertices.Add(v1);
		vertices.Add(v3);
		vertices.Add(v2);

		normals.Add(CalculateNormal(v1, v3, v2));
		normals.Add(CalculateNormal(v3, v2, v1));
		normals.Add(CalculateNormal(v2, v1, v3));

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
	}

	public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
	{
		uvs.Add(uv1);
		uvs.Add(uv3);
		uvs.Add(uv2);
	}

	public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3)
	{
		uv2s.Add(uv1);
		uv2s.Add(uv3);
		uv2s.Add(uv2);
	}

	public void AddTriangleCellData(Vector3 indices, Color weights1, Color weights2, Color weights3)
	{
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		
		cellWeights.Add(weights1);
		cellWeights.Add(weights3);
		cellWeights.Add(weights2);
	}

	public void AddTriangleCellData(Vector3 indices, Color weights)
	{
		AddTriangleCellData(indices, weights, weights, weights);
	}

	public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		int vertexIndex = vertices.Count;

		v1 = Metrics.Perturb(v1);
		v2 = Metrics.Perturb(v2);
		v3 = Metrics.Perturb(v3);
		v4 = Metrics.Perturb(v4);

		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);

		vertices.Add(v3);
		vertices.Add(v2);
		vertices.Add(v4);

		normals.Add(CalculateNormal(v1, v3, v2));
		normals.Add(CalculateNormal(v2, v1, v3));
		normals.Add(CalculateNormal(v3, v2, v1));

		normals.Add(CalculateNormal(v3, v4, v2));
		normals.Add(CalculateNormal(v2, v3, v4));
		normals.Add(CalculateNormal(v4, v2, v3));

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		int vertexIndex = vertices.Count;

		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);

		vertices.Add(v3);
		vertices.Add(v2);
		vertices.Add(v4);

		normals.Add(CalculateNormal(v1, v3, v2));
		normals.Add(CalculateNormal(v2, v1, v3));
		normals.Add(CalculateNormal(v3, v2, v1));

		normals.Add(CalculateNormal(v3, v4, v2));
		normals.Add(CalculateNormal(v2, v3, v4));
		normals.Add(CalculateNormal(v4, v2, v3));

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
	{
		Vector2 v1 = new Vector2(uMin, vMin);
		Vector2 v2 = new Vector2(uMax, vMin);
		Vector2 v3 = new Vector2(uMin, vMax);
		Vector2 v4 = new Vector2(uMax, vMax);

		AddQuadUV(v1, v2, v3, v4);
	}

	public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
	{
		Vector2 v1 = new Vector2(uMin, vMin);
		Vector2 v2 = new Vector2(uMax, vMin);
		Vector2 v3 = new Vector2(uMin, vMax);
		Vector2 v4 = new Vector2(uMax, vMax);

		AddQuadUV2(v1, v2, v3, v4);
	}

	public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
	{
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
		uvs.Add(uv3);
		uvs.Add(uv2);
		uvs.Add(uv4);
	}

	public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
	{
		uv2s.Add(uv1);
		uv2s.Add(uv2);
		uv2s.Add(uv3);
		uv2s.Add(uv3);
		uv2s.Add(uv2);
		uv2s.Add(uv4);
	}

	public void AddQuadCellData(
		Vector3 indices,
		Color weights1, Color weights2, Color weights3, Color weights4)
	{
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);

		cellWeights.Add(weights1);
		cellWeights.Add(weights2);
		cellWeights.Add(weights3);
		cellWeights.Add(weights3);
		cellWeights.Add(weights2);
		cellWeights.Add(weights4);
	}

	public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2)
	{
		AddQuadCellData(indices, weights1, weights1, weights2, weights2);
	}

	public void AddQuadCellData(Vector3 indices, Color weights)
	{
		AddQuadCellData(indices, weights, weights, weights, weights);
	}

	public Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 normal = (c - b).Cross(a - b).Normalized();
		if (normal.y < 0)
		{
			normal = -normal;
		}
		return normal;
	}
}
