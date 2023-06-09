using System;
using System.Collections.Generic;
using TerrainPathfindingKit.PathGrids;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Caches
{
	/// <summary>
	/// Global cache containing pathfinding info of any pawn regardless of their map.
	/// See TerrainPathing for details.
	/// </summary>
	public static class PawnPathingCache
	{
		/// <summary>
		/// Indexed by the Pawn's thingIDNumber. Only contains pawns using a non-default PathingType.
		/// </summary>
		private static readonly Dictionary<int, PathingContext> PathingContextSet = new Dictionary<int, PathingContext>();

		/// <summary>
		/// Indexed by the Pawn's thingIDNumber. Only contains pawns using a non-default PathingType.
		/// </summary>
		private static readonly Dictionary<int, TerrainPathGrid>
			TerrainPathGridSet = new Dictionary<int, TerrainPathGrid>();

		/// <summary>
		/// Updates the cache after a potential PathingType change in a pawn.
		/// ToDo: document update conditions.
		/// </summary>
		/// <param name="pawn">Pawn to update.</param>
		public static void Update(Pawn pawn)
		{
			Remove(pawn);
			var terrainPathing = TerrainPathingCache.Get(pawn.Map);
			if (terrainPathing != null)
			{
				var pathingType = PawnPathingTypeUtil.For(pawn);
				Logging.Error($"Updating pathing cache for {pawn}: {Enum.GetName(typeof(PathingType), pathingType)}");
				var id = pawn.thingIDNumber;
				if (pathingType != PathingType.Default)
				{
					PathingContextSet[id] = terrainPathing.ContextFor(pathingType);
					TerrainPathGridSet[id] = terrainPathing.GridFor(pathingType);
				}
			}
		}

		/// <summary>
		/// Removes a pawn from the cache after it despawns.
		/// </summary>
		/// <param name="pawn">Pawn being removed.</param>
		public static void Remove(Pawn pawn)
		{
			var id = pawn.thingIDNumber;
			PathingContextSet.Remove(id);
			TerrainPathGridSet.Remove(id);
		}

		/// <summary>
		/// PathingContext to use, or null for pawns using PathingType.Default.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>PathingContext to use, or null for pawns using PathingType.Default.</returns>
		public static PathingContext ContextFor(Pawn pawn)
		{
			return PathingContextSet.TryGetValue(pawn.thingIDNumber, out PathingContext value) ? value : null;
		}

		/// <summary>
		/// TerrainPathGrid to use, or null for pawns using PathingType.Default.
		/// </summary>
		/// <param name="pawn">Pawn to check.</param>
		/// <returns>TerrainPathGrid to use, or null for pawns using PathingType.Default.</returns>
		public static TerrainPathGrid GridFor(Pawn pawn)
		{
			return TerrainPathGridSet.TryGetValue(pawn.thingIDNumber, out TerrainPathGrid value) ? value : null;
		}
	}
}