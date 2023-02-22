using Godot;

namespace Haldric;

public partial class ScenarioHud : CanvasLayer
{
	[Export] Label _terrainLabel = default!;

	public void UpdateTileInfo(Tile tile)
	{
		var overlay = tile.OverlayTerrain is null ? "" : $"^{tile.OverlayTerrain.Code}";
		var text = $"{tile.Coords}\n{tile.BaseTerrain.Code}{overlay}";
		_terrainLabel.Text = text;
	}
}
