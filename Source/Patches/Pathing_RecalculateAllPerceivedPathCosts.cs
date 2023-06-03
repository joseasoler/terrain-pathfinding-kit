using HarmonyLib;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Recalculate terrain path grids after vanilla ones.
	/// </summary>
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.RecalculateAllPerceivedPathCosts))]
	internal static class Pathing_RecalculateAllPerceivedPathCosts
	{
		internal static void Postfix(Pathing __instance)
		{
			__instance.Normal.map.GetComponent<TerrainPathing>().RecalculateAllPerceivedPathCosts();
		}
	}
}