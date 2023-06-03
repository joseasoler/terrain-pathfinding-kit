using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.CommonGrids
{
	/// <summary>
	///
	/// This is how thing costs and ignore repeaters are evaluated in vanilla:
	/// * Valid previous cell with an ignore repeater: thing cost is the sum of all non-ignore repeat things.
	/// * Invalid previous cell or it does not have an ignore repeater: thing cost is the sum of all things.
	/// </summary>
	public class ThingsGrid
	{
		private readonly Map _map;

		/// <summary>
		/// Total cost of all things in the cell.
		/// </summary>
		private readonly int[] _cost;

		/// <summary>
		/// Total cost of all things with IsPathCostIgnoreRepeater returning false in the cell.
		/// </summary>
		private readonly int[] _nonIgnoreRepeaterCost;

		/// <summary>
		/// True iff the cell contains one or more things with IsPathCostIgnoreRepeater returning true.
		/// </summary>
		private readonly bool[] _hasIgnoreRepeater;

		/// <summary>
		/// True for cells containing a door.
		/// </summary>
		private readonly bool[] _doors;

		public ThingsGrid(Map map)
		{
			_map = map;
			var numCells = _map.cellIndices.NumGridCells;
			_cost = new int[numCells];
			_nonIgnoreRepeaterCost = new int[numCells];
			_hasIgnoreRepeater = new bool[numCells];
			_doors = new bool[numCells];
		}

		public void Update(IntVec3 cell)
		{
			if (!cell.InBounds(_map))
			{
				return;
			}

			var cellIndex = _map.cellIndices.CellToIndex(cell);
			// Get references to the relevant values of this cell.
			ref int cost = ref _cost[cellIndex];
			ref int nonIgnoreRepeaterCost = ref _nonIgnoreRepeaterCost[cellIndex];
			ref bool ignoreRepeater = ref _hasIgnoreRepeater[cellIndex];
			ref bool door = ref _doors[cellIndex];
			// Reset the values.
			cost = 0;
			nonIgnoreRepeaterCost = 0;
			ignoreRepeater = false;
			door = false;

			List<Thing> thingList = _map.thingGrid.ThingsListAtFast(cell);
			for (int thingIndex = 0; thingIndex < thingList.Count; ++thingIndex)
			{
				var thing = thingList[thingIndex];
				if (thing.def.passability == Traversability.Impassable)
				{
					cost = PathGrid.ImpassableCost;
					nonIgnoreRepeaterCost = PathGrid.ImpassableCost;
					break;
				}

				var thingCost = thing.def.pathCost;
				cost = Math.Max(cost, thingCost);

				if (!PathGrid.IsPathCostIgnoreRepeater(thing.def))
				{
					nonIgnoreRepeaterCost = Math.Max(nonIgnoreRepeaterCost, thingCost);
				}
				else
				{
					ignoreRepeater = true;
				}

				door = door || thing is Building_Door;
			}
		}

		public int CostAt(int cellIndex)
		{
			return _cost[cellIndex];
		}

		public bool HasIgnoreRepeater(int cellIndex)
		{
			return _hasIgnoreRepeater[cellIndex];
		}

		public int NonIgnoreRepeaterCostAt(int cellIndex)
		{
			return _nonIgnoreRepeaterCost[cellIndex];
		}

		public bool HasDoor(int cellIndex)
		{
			return _doors[cellIndex];
		}
	}
}