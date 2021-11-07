using Godot;
using System.Collections.Generic;

public partial class TerrainHighlighter : Node3D
{
	[Export] public PackedScene TerrainHighlight;
	public List<Border3D> Path { get; set; } = new List<Border3D>();

	public void ShowPath(Path path)
	{
		foreach (var child in Path)
		{
			RemoveChild(child);
			child.QueueFree();
		}

		Path.Clear();

		foreach(var pLocEntity in path.Checkpoints)
		{
			ref var coords = ref pLocEntity.Get<Coords>();
			ref var elevation = ref pLocEntity.Get<Elevation>();

			var pos = coords.World;
			pos.y = elevation.Height;

			//PlaceHighlight(pos, new Color("FFFFFF"), 0.7f, true);
		}
	}

	public void PlaceHighlight(Vector3 position, Color color, float scaleFactor, bool isPath = false)
	{
		var highlight = TerrainHighlight.Instantiate<Border3D>();
		highlight.Color = color;
		//highlight.ScaleFactor = scaleFactor;
		highlight.Position = position;
		AddChild(highlight);

		if (isPath)
		{
			Path.Add(highlight);
		}
	}

	public void PlaceBorder(Vector3 position, Color color, float rotate)
	{
		Border3D highlight = TerrainHighlight.Instantiate<Border3D>();
		highlight.Color = color;
		highlight.Direction = rotate;
		highlight.Position = position;
		AddChild(highlight);
	}

	public void Clear()
	{
		foreach (Node child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}

		Path.Clear();
	}
}
