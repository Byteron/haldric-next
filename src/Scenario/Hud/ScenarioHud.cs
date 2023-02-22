using Godot;

namespace Haldric;

public partial class ScenarioHud : CanvasLayer
{
	[Export] Label _terrainLabel = default!;
	[Export] Label _unitLabel = default!;

	public void UpdateTileInfo(Tile tile)
	{
		var overlay = tile.OverlayTerrain is null ? "" : $"^{tile.OverlayTerrain.Code}";
		var text = $"{tile.Coords}\n{tile.BaseTerrain.Code}{overlay}";
		_terrainLabel.Text = text;
	}

	public void UpdateUnitInfo(Unit unit)
	{
		var text = $"{unit.Name}\n{unit.Coords}";
		_unitLabel.Text = text;
	}

	public void ClearUnitInfo()
	{
		_unitLabel.Text = string.Empty;
	}
}
