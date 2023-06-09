using HarmonyLib;
using RimWorld;
using TerrainPathfindingKit.Caches;
using Verse;

namespace TerrainPathfindingKit.Patches
{
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.GetAvoidGrid))]
	internal static class PawnUtility_GetAvoidGrid
	{
		internal static void Postfix(ref ByteGrid __result, Pawn p)
		{
			var grid = PawnPathingCache.GridFor(p);
			if (grid != null)
			{
				__result = grid.AvoidGrid(__result);
			}
		}
	}
}