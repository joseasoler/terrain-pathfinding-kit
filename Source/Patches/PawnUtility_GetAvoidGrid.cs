using HarmonyLib;
using RimWorld;
using Verse;

namespace TerrainPathfindingKit.Patches
{
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.GetAvoidGrid))]
	internal static class PawnUtility_GetAvoidGrid
	{
		internal static void Postfix(ref ByteGrid __result, Pawn p)
		{
			var terrainPathing = Getter.GetTerrainPathing(p.Map);
			var pathingType = terrainPathing.TypeFor(p);
			var grid = terrainPathing.GridFor(pathingType);
			if (grid != null)
			{
				__result = grid.AvoidGrid(__result);
			}
		}
	}
}