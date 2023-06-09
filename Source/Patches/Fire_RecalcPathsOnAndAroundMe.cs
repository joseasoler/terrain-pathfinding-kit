using HarmonyLib;
using RimWorld;
using TerrainPathfindingKit.Caches;

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
			TerrainPathingCache.Get(__instance.Map).UpdateFire(__instance.Position, __instance.Spawned);
		}
	}
}