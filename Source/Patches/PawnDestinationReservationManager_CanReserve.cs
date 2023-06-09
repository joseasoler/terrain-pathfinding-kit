using HarmonyLib;
using TerrainPathfindingKit.Caches;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Prevent pawns from reserving cells they cannot enter.
	/// </summary>
	[HarmonyPatch(typeof(PawnDestinationReservationManager), nameof(PawnDestinationReservationManager.CanReserve))]
	internal static class PawnDestinationReservationManager_CanReserve
	{
		internal static bool Prefix(ref bool __result, IntVec3 c, Pawn searcher, bool draftedOnly)
		{
			var grid = PawnPathingCache.GridFor(searcher);
			if (grid != null && !grid.CanEnterCell(c))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}