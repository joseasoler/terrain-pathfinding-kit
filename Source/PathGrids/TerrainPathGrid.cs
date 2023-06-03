using System;
using TerrainPathfindingKit.CommonGrids;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.PathGrids
{
	/// <summary>
	/// Wraps a path grid with an alternate recalculation logic.
	/// </summary>
	public abstract class TerrainPathGrid
	{
		protected readonly Map Map;

		/// <summary>
		/// Wrapped PathGrid instance. The values in this object are calculated by TerrainPathGrid. Then, it is handed over
		/// to pathfinding code expecting a PathGrid to use the calculated values.
		/// </summary>
		public readonly PathGrid Grid;

		/// <summary>
		/// Pathfinding costs of current fires.
		/// </summary>
		private readonly FireGrid _fires;

		/// <summary>
		/// Pathfinding costs of current things.
		/// </summary>
		private readonly ThingsGrid _things;

		/// <summary>
		/// Construct a new path grid from scratch.
		/// </summary>
		/// <param name="map">Map for the path grid.</param>
		/// <param name="fires">Pathfinding costs of fire.</param>
		/// <param name="things">Pathfinding costs of things.</param>
		public TerrainPathGrid(Map map, FireGrid fires, ThingsGrid things)
		{
			Map = map;
			Grid = new PathGrid(map, true);
			_fires = fires;
			_things = things;
		}

		public void RecalculatePerceivedPathCostAt(IntVec3 c, ref bool haveNotified)
		{
			if (!c.InBounds(Map))
			{
				return;
			}

			bool wasWalkable = Grid.WalkableFast(c);
			UpdateCalculatedCostAt(c);
			if (haveNotified || Grid.WalkableFast(c) == wasWalkable)
			{
				return;
			}

			Map.reachability.ClearCache();
			Map.regionDirtyer.Notify_WalkabilityChanged(c, !wasWalkable);
			haveNotified = true;
		}

		public void RecalculateAllPerceivedPathCosts()
		{
			bool haveNotified = false;
			foreach (IntVec3 allCell in Map.AllCells)
			{
				RecalculatePerceivedPathCostAt(allCell, ref haveNotified);
			}
		}

		/// <summary>
		/// Updates all cached pathfinding costs. The internal PathGrid is kept with the values expected by
		/// PathFinder.FindPath and other places that use the underlying pathGrid array directly.
		/// </summary>
		/// <param name="cell">Cell for which the costs are being calculated. Assumed to be in bounds.</param>
		/// <returns>New cost in this cell.</returns>
		private void UpdateCalculatedCostAt(IntVec3 cell)
		{
			// Total should be equal to calling CalculatedCostAt with perceivedStatic = true and an invalid prevCell.
			var cellIndex = Map.cellIndices.CellToIndex(cell);
			ref var totalCost = ref Grid.pathGrid[cellIndex];
			totalCost = TerrainCostAt(cellIndex);
			if (totalCost < PathGrid.ImpassableCost)
			{
				// Invalid prevCell, do not check ignore repeaters.
				totalCost = Math.Max(totalCost, _things.CostAt(cellIndex));
			}

			int snowCost = SnowUtility.MovementTicksAddOn(Map.snowGrid.GetCategory(cell));
			totalCost = Math.Max(totalCost, snowCost);
			totalCost += _fires.CostAt(cellIndex); // perceivedStatic = true

			// Update the custom avoid grid.
			UpdateAvoidGridCell(cellIndex);
		}

		/// <summary>
		/// Final cost of moving between two cells.
		/// These values match the expectations of Pawn_PathFollower.CostToMoveIntoCell.
		/// </summary>
		/// <param name="cell">Destination cell</param>
		/// <param name="prevCell">Origin cell</param>
		/// <returns>Cost of moving between the cells.</returns>
		public int CostToMoveIntoCell(IntVec3 cell, IntVec3 prevCell)
		{
			// Equivalent of calling CalculatedCostAt with perceivedStatic = false and a valid prevCell.
			// Total should be equal to calling CalculatedCostAt with perceivedStatic = false and a valid prevCell.
			var cellIndex = Map.cellIndices.CellToIndex(cell);
			var totalCost = TerrainCostAt(cellIndex);
			var prevCellIndex = Map.cellIndices.CellToIndex(prevCell);
			if (totalCost < PathGrid.ImpassableCost)
			{
				var prevIsIgnoreRepeater = prevCell.IsValid && _things.HasIgnoreRepeater(prevCellIndex);
				var thingCost = prevIsIgnoreRepeater ? _things.NonIgnoreRepeaterCostAt(cellIndex) : _things.CostAt(cellIndex);
				totalCost = Math.Max(totalCost, thingCost);
			}

			int snowCost = SnowUtility.MovementTicksAddOn(Map.snowGrid.GetCategory(cell));
			totalCost = Math.Max(totalCost, snowCost);
			if (prevCell.IsValid && _things.HasDoor(prevCellIndex) && _things.HasDoor(cellIndex))
			{
				totalCost += 45;
			}

			return totalCost;
		}

		public virtual int ExtraDraftedPerceivedPathCost(TerrainDef def)
		{
			return def.extraDraftedPerceivedPathCost;
		}

		public virtual int ExtraNonDraftedPerceivedPathCost(TerrainDef def)
		{
			return def.extraNonDraftedPerceivedPathCost;
		}

		protected abstract int TerrainCostAt(int cellIndex);

		/// <summary>
		/// AvoidGrid to use with this path grid.
		/// </summary>
		/// <param name="defaultGrid">Vanilla path grid.</param>
		/// <returns>A potentially customized avoid grid.</returns>
		public virtual ByteGrid AvoidGrid(ByteGrid defaultGrid)
		{
			return defaultGrid;
		}

		/// <summary>
		/// Updates the custom avoid grid after a change a path grid change.
		/// </summary>
		/// <param name="cellIndex">Cell modified in the path grid.</param>
		public virtual void UpdateAvoidGridCell(int cellIndex)
		{
		}

		/// <summary>
		/// Regenerate the custom avoid grid after a vanilla avoid grid update.
		/// </summary>
		public virtual void RegenerateAvoidGrid()
		{
		}
	}
}