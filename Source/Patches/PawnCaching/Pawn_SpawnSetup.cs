using HarmonyLib;
using TerrainPathfindingKit.Caches;
using Verse;

namespace TerrainPathfindingKit.Patches.PawnCaching
{
	/// <summary>
	/// Add the pawn to the PawnPathingCache.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	internal static class Pawn_SpawnSetup
	{
		internal static void Postfix(ref Pawn __instance)
		{
			if (!__instance.Spawned)
			{
				return;
			}

			PawnPathingCache.Update(__instance);
		}
	}
}