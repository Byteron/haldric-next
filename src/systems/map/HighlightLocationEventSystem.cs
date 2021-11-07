using Bitron.Ecs;
using Godot;
using System.Collections.Generic;
public struct HighlightLocationEvent
{
	public Coords Coords { get; set; }
	public int Range { get; set; }

	public HighlightLocationEvent(Coords coords, int range)
	{
		Coords = coords;
		Range = range;
	}
}

public class HighlightLocationsEventSystem : IEcsSystem
{
	private Dictionary<Direction, float> _rotations = new Dictionary<Direction, float>() {
		{ Direction.W, 0 },
		{ Direction.NW, Godot.Mathf.Pi/3 },
		{ Direction.NE, 2 * Godot.Mathf.Pi/3 },
		{ Direction.E, Godot.Mathf.Pi },
		{ Direction.SE, - Godot.Mathf.Pi/3 },
		{ Direction.SW, - 2 * Godot.Mathf.Pi/3 }
	};
	public void Run(EcsWorld world)
	{
		var query = world.Query<HighlightLocationEvent>().End();

		foreach (var eventEntityId in query)
		{
			var map = world.GetResource<Map>();
			var grid = map.Grid;

			var eventData = world.Entity(eventEntityId).Get<HighlightLocationEvent>();
			
			var locEntity = map.Locations.Get(eventData.Coords.Cube);
			ref var unit = ref locEntity.Get<HasUnit>();
			var unitEntity = unit.Entity;
			ref var side = ref unitEntity.Get<Side>();
			ref var attacks = ref unitEntity.Get<Attacks>();
			
			var terrainHighlighter = world.GetResource<TerrainHighlighter>();
			terrainHighlighter.Clear();
			
			var maxAttackRange = attacks.GetMaxAttackRange();
			var cellsInAttackRange = Hex.GetCellsInRange(eventData.Coords.Cube, maxAttackRange);

			foreach (var cCell in cellsInAttackRange)
			{
				var nCoords = Coords.FromCube(cCell);
				if (!grid.IsCoordsInGrid(nCoords))
				{
					continue;
				}

				var nLocEntity = map.Locations.Dict[nCoords.Cube];

				var attackRange = map.GetEffectiveAttackDistance(eventData.Coords, nCoords);
				var attackerBonusAttackRange = map.GetBonusAttackRange(eventData.Coords, nCoords);

				var attack = attacks.GetUsableAttack(attackRange, attackerBonusAttackRange);

				ref var nElevation = ref nLocEntity.Get<Elevation>();
				
				var position = nCoords.World;
				position.y = nElevation.Height + 0.1f;
				
				var hasUnit = nLocEntity.Has<HasUnit>();

				if (hasUnit)
				{
					if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value)
					{
						continue;
					}
				}

				if (attack.IsAlive())
				{
					if (attackRange > 1)
					{
						//terrainHighlighter.PlaceHighlight(position, new Color("881111"), 0.6f);
					}
					else
					{
						//terrainHighlighter.PlaceHighlight(position, new Color("774411"), 0.6f);
					}
				}
			}

			Vector3[] cellsInMoveRange = Hex.GetCellsInRange(eventData.Coords.Cube, eventData.Range);
			List<Coords> filteredList = new List<Coords>();

			foreach (Vector3 cCell in cellsInMoveRange)
			{
				Coords nCoords = Coords.FromCube(cCell);
				if (!grid.IsCoordsInGrid(nCoords))
				{
					continue;
				}

				var nLocEntity = map.Locations.Dict[cCell];

				if (nLocEntity.Get<Distance>().Value > eventData.Range)
				{
					continue;
				}

				ref var nElevation = ref nLocEntity.Get<Elevation>();
				var hasUnit = nLocEntity.Has<HasUnit>();

				if (hasUnit)
				{
					if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value)
					{
						continue;
					}
				}

				if (hasUnit)
				{
					continue;
				}

				Vector3 position = nCoords.World;
				position.y = nElevation.Height + 0.1f;
				bool isEdge = false;
				Vector3[] Neighbors = Hex.GetNeighbors(nCoords.Cube);
				GD.Print(nCoords.ToString());
				for (Direction direction = Direction.NE; direction <= Direction.SE; direction++) {
					Vector3 cNeighbor = Neighbors[(int)direction];
					Coords nNeighbor = Coords.FromCube(cNeighbor);
					bool validCoord = grid.IsCoordsInGrid(nNeighbor);
					bool inRange = map.Locations.Dict[cNeighbor].Get<Distance>().Value <= eventData.Range;
					if (!validCoord || !inRange) {
						isEdge = true;
						terrainHighlighter.PlaceBorder(position, new Color("111188"), _rotations[direction]);
						GD.Print($"- {direction}");
					}
				}
				if (!isEdge) {
					continue;
				}
			}
		}
	}
}
