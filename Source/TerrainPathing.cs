using System.Collections.Generic;
using TerrainPathfindingKit.Caches;
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
		private readonly FireGrid _fires;
		private readonly ThingsGrid _things;

		private AquaticPathGrid _aquaticGrid;

		/// <summary>
		/// Pre-initialized pathing contexts to avoid extra allocations.
		/// </summary>
		private List<PathingContext> _contexts;

		public TerrainPathing(Map map) : base(map)
		{
			_fires = new FireGrid(map);
			_things = new ThingsGrid(map);
			_aquaticGrid = new AquaticPathGrid(map, _fires, _things);
			_contexts = new List<PathingContext>
			{
				null, // PathingType.Default.
				new PathingContext(map, _aquaticGrid.Grid)
			};
		}

		/// <summary>
		/// After the map is generated or loaded, start accepting pathfinding update calls.
		/// </summary>
		public override void FinalizeInit()
		{
			TerrainPathingCache.Add(map, this);
			RecalculateAllPerceivedPathCosts();
		}

		/// <summary>
		/// Clean up the map component cache and stop accepting pathfinding update calls.
		/// </summary>
		public override void MapRemoved()
		{
			TerrainPathingCache.Remove(map);
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
			_things.Update(cell);
			_aquaticGrid.RecalculatePerceivedPathCostAt(cell, ref haveNotified);
		}

		/// <summary>
		/// Update the fire grid after a fire instance is created or removed.
		/// </summary>
		/// <param name="position">Position of the fire.</param>
		/// <param name="spawned">True iff the fire has just spawned. False if it is being destroyed.</param>
		public void UpdateFire(IntVec3 position, bool spawned)
		{
			_fires.Update(position, spawned);
		}

		public void RegenerateAvoidGrids()
		{
			_aquaticGrid.RegenerateAvoidGrid();
		}
	}
}