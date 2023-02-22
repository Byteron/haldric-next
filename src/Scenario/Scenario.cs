using System.Net.Security;
using Godot;

namespace Haldric;

public partial class Scenario : Node3D
{
	Map _map = default!;
	
	[Export] ScenarioHud _hud = default!;
	[Export] PackedScene _mapScene = default!;
	[Export] PackedScene _unitScene = default!;
	[Export] CameraOperator _cameraOperator = default!;

	Tile? _selectedTile;

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

		if (e.IsActionPressed("select_unit"))
		{
			SelectTile();
		}

		if (e.IsActionPressed("deselect_unit"))
		{
			DeselectTile();
		}
	}

	public override void _Ready()
	{
		SpawnMap("Valley");
	}

	void DeselectTile()
	{
		GD.Print("Tile Deselected");
		_selectedTile = null;
	}
	
	void SelectTile()
	{
		GD.Print("Tile Selected");
		_selectedTile = _map.HoveredTile;
	}
	
	void SpawnMap(string name)
	{
		_map = _mapScene.Instantiate<Map>();
		_map.TileHovered += OnMapTileHovered;
		AddChild(_map);
		
		_map.Initialize(Data.Instance.Maps[name]);
		
		SpawnUnit(Coords.FromXZ(5, 5));
		SpawnUnit(Coords.FromXZ(7, 7));
		SpawnUnit(Coords.FromXZ(12, 12));
	}

	void SpawnUnit(Coords coords)
	{
		var tile = _map.GetTile(coords);

		if (tile.Unit is not null)
		{
			GD.PushWarning("Tile already has a Unit");
			return;
		}
		
		var unit = _unitScene.Instantiate<Unit>();
		_map.AddChild(unit);
		
		unit.Coords = coords;
		unit.Position = tile.WorldPosition;

		tile.Unit = unit;
	}

	void OnMapTileHovered(Tile tile)
	{
		_hud.UpdateTileInfo(tile);

		if (tile.Unit is not null)
		{
			_hud.UpdateUnitInfo(tile.Unit);
		}
		else
		{
			_hud.ClearUnitInfo();
		}
	}
}