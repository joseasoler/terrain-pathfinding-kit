using Verse;

namespace TerrainPathfindingKit.CommonGrids
{
	/// <summary>
	/// Keeps track of pathfinding costs associated with fire presence.
	/// In vanilla, PathGrid.CalculatedCostAt calculates this cost when perceivedStatic is set to true.
	/// </summary>
	public class FireGrid
	{
		private readonly Map _map;

		private readonly int[] _cost;

		public FireGrid(Map map)
		{
			_map = map;
			_cost = new int[_map.cellIndices.NumGridCells];
		}

		public void Update(IntVec3 cell, bool spawned)
		{
			if (!cell.InBounds(_map))
			{
				return;
			}

			const int centerCellCost = 1000;
			_cost[_map.cellIndices.CellToIndex(cell)] += spawned ? centerCellCost : -centerCellCost;

			var adjacentCells = GenAdj.AdjacentCells;
			for (int adjacentIndex = 0; adjacentIndex < adjacentCells.Length; ++adjacentIndex)
			{
				IntVec3 adjacentCell = adjacentCells[adjacentIndex];
				const int adjacentCellCost = 150;
				_cost[_map.cellIndices.CellToIndex(adjacentCell)] += spawned ? adjacentCellCost : -adjacentCellCost;
			}
		}

		public int CostAt(int cellIndex)
		{
			return _cost[cellIndex];
		}
	}
}