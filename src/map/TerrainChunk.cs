using Godot;
using Leopotam.Ecs;

public partial class TerrainChunk : Node3D
{
    private static Color Weight1 = new Color(1f, 0f, 0f);
	private static Color Weight2 = new Color(0f, 1f, 0f);
	private static Color Weight3 = new Color(0f, 0f, 1f);

    private bool _enabled = false;

    private Locations _locations;

    private TerrainMesh _terrain;

    public override void _Ready()
    {
        _terrain = GetNode<TerrainMesh>("TerrainMesh");
    }

    public void Build(Locations locations)
	{
        _locations = locations;
		_enabled = true;
		CallDeferred("Triangulate");
	}

    private void Triangulate()
	{
		if (_enabled)
		{
			Triangulate(_locations);
		}

		_enabled = false;
	}

    private void Triangulate(Locations locations)
    {
        _terrain.Clear();
        
        foreach (var item in locations.Dict)
        {
            Triangulate(item.Value);
        }

        _terrain.Apply();
    }

    private void Triangulate(EcsEntity entity)
    {
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
		{
			Triangulate(direction, entity);
		}
    }

    private void Triangulate(Direction direction, EcsEntity entity)
    {
        ref var coords = ref entity.Get<Coords>();
        Vector3 center = coords.World;

        Vector3 v1 = center + Metrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + Metrics.GetSecondSolidCorner(direction);

        _terrain.AddTriangle(center, v1, v2);

        TriangulateConnection(direction, entity, v1, v2);
    }

    private void TriangulateConnection(Direction direction, EcsEntity entity, Vector3 v1, Vector3 v2)
    {
        ref var neighbors = ref entity.Get<Neighbors>();

        if (!neighbors.Has(direction))
        {
            return;
        }

        var nEntity = neighbors.Get(direction);

        var v3 = v1 + Metrics.GetBridge(direction);
        var v4 = v2 + Metrics.GetBridge(direction);

        _terrain.AddQuad(v1, v2, v3, v4);

        if (!neighbors.Has(direction.Next()) || direction > Direction.E)
        {
            return;
        }

        _terrain.AddTriangle(v2, v4, v2 + Metrics.GetBridge(direction.Next()));
    }
}