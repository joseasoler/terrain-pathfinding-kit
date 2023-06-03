using HarmonyLib;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Replace the pathing context of a pawn when necessary.
	/// </summary>
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.For), typeof(Pawn))]
	internal static class Pathing_For_Pawn
	{
		internal static bool Prefix(Pathing __instance, ref PathingContext __result, Pawn pawn)
		{
			return Pathing_For_Util.Prefix(__instance, ref __result, pawn);
		}
	}
}