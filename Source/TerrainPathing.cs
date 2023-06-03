using System;
using System.Collections.Generic;
using TerrainPathfindingKit.CommonGrids;
using TerrainPathfindingKit.PathGrids;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit
{
	/// <summary>
	/// Stores and keeps different path grids up to date. Returns the correct path grid for each pawn.
	/// </summary>
	public class TerrainPathing : MapComponent
	{
		public FireGrid Fires;
		public ThingsGrid Things;

		private PawnPathingType _pawnPathing = new PawnPathingType();

		private TerrainPathGrid _aquaticGrid;

		/// <summary>
		/// Pre-initialized pathing contexts to avoid extra allocations.
		/// </summary>
		private List<PathingContext> _contexts;

		public TerrainPathing(Map map) : base(map)
		{
			Fires = new FireGrid(map);
			Things = new ThingsGrid(map);
			_aquaticGrid = new AquaticPathGrid(map, Fires, Things);
			_contexts = new List<PathingContext>();
			_contexts.Add(null); // PathingType.Default.
			_contexts.Add(new PathingContext(map, _aquaticGrid.Grid));
		}

		public PathingType TypeFor(Pawn pawn)
		{
			return _pawnPathing.For(pawn);
		}

		/// <summary>
		/// Context to use for a pathing type.
		/// </summary>
		/// <param name="type">Pathing type of the pawn being checked.</param>
		/// <returns>Pathing context to use for this type. Null means use vanilla code.</returns>
		public PathingContext ContextFor(PathingType type)
		{
			return _contexts[(int) type];
		}

		public TerrainPathGrid GridFor(PathingType type)
		{
			TerrainPathGrid grid = null;
			switch (type)
			{
				case PathingType.Aquatic:
					grid = _aquaticGrid;
					break;
				case PathingType.Default:
					break;
				case PathingType.Count:
				default:
					Logging.ErrorOnce("GridFor: unsupported PathingType.");
					break;
			}

			return grid;
		}

		/// <summary>
		/// Calculate the perceived path costs of every cell in the map.
		/// </summary>
		public void RecalculateAllPerceivedPathCosts()
		{
			_aquaticGrid.RecalculateAllPerceivedPathCosts();
		}

		/// <summary>
		/// Recalculate the perceived costs of a specific cell, keeping the other costs the same.
		/// </summary>
		/// <param name="cell">Cell to be recalculated.</param>
		/// <param name="haveNotified">True if a past Path Grid already needed to notify changes.</param>
		public void RecalculatePerceivedPathCostAt(IntVec3 cell, ref bool haveNotified)
		{
			_aquaticGrid.RecalculatePerceivedPathCostAt(cell, ref haveNotified);
		}
	}
}