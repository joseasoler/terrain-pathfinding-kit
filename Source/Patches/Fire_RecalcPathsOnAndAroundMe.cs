using HarmonyLib;
using RimWorld;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Keep FireGrid updated.
	/// </summary>
	[HarmonyPatch(typeof(Fire), "RecalcPathsOnAndAroundMe")]
	internal static class Fire_RecalcPathsOnAndAroundMe
	{
		internal static void Prefix(Fire __instance)
		{
			__instance.Map.GetComponent<TerrainPathing>().Fires.Update(__instance.Position, __instance.Spawned);
		}
	}
}