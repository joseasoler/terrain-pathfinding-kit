using HarmonyLib;
using TerrainPathfindingKit.Caches;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Make pawns avoid cells they should not be in.
	/// </summary>
	[HarmonyPatch(typeof(Reachability), nameof(Reachability.CanReach), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(PathEndMode), typeof(TraverseParms))]
	internal static class Reachability_CanReach
	{
		internal static void Postfix(ref bool __result, Map ___map, IntVec3 start, LocalTargetInfo dest, PathEndMode peMode,
			TraverseParms traverseParams)
		{
			if (__result && traverseParams.pawn != null)
			{
				var grid = PawnPathingCache.GridFor(traverseParams.pawn);
				if (grid != null)
				{
					__result = grid.CanEnterCell(dest.Cell);
				}
			}
		}
	}
}