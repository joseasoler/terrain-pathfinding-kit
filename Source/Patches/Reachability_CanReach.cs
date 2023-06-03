using HarmonyLib;
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
				var terrainPathing = Getter.GetTerrainPathing(___map);
				var pathingType = terrainPathing.TypeFor(traverseParams.pawn);
				var grid = terrainPathing.GridFor(pathingType);
				if (grid != null)
				{
					__result = grid.CanEnterCell(dest.Cell);
				}
			}
		}
	}
}