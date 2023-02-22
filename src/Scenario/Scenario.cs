using Godot;

namespace Haldric;

public partial class Scenario : Node3D
{
	Map _map = default!;
	
	[Export] ScenarioHud _hud = default!;
	[Export] PackedScene _mapScene = default!;
	[Export] CameraOperator _cameraOperator = default!;

	public override void _Input(InputEvent e)
	{
		if (e.IsActionPressed("ui_cancel"))
		{
			GetTree().ChangeSceneToFile("res://src/Menu/MainMenu.tscn");
		}
	
		if (e.IsActionPressed("ui_left"))
		{
			_map.QueueFree();
			SpawnMap("Valley");
		}
		
		if (e.IsActionPressed("ui_right"))
		{
			_map.QueueFree();
			SpawnMap("Triplet");
		}
	}

	public override void _Ready()
	{
		SpawnMap("Valley");
	}

	void SpawnMap(string name)
	{
		_map = _mapScene.Instantiate<Map>();
		_map.TileHovered += OnMapTileHovered;
		AddChild(_map);
		
		_map.Initialize(Data.Instance.Maps[name]);
	}

	void OnMapTileHovered(Tile tile)
	{
		GD.Print("On Map Hovered");
		_hud.UpdateTileInfo(tile);
	}
}