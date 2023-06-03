using HarmonyLib;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Regenerate custom terrain AvoidGrids when necessary.
	/// </summary>
	[HarmonyPatch(typeof(AvoidGrid), nameof(AvoidGrid.Regenerate))]
	internal static class AvoidGrid_Regenerate
	{
		internal static void Postfix(AvoidGrid __instance)
		{
			var terrainPathing = Getter.GetTerrainPathing(__instance.map);
			terrainPathing.RegenerateAvoidGrids();
		}
	}
}