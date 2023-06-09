using HarmonyLib;
using TerrainPathfindingKit.Caches;
using Verse;

namespace TerrainPathfindingKit.Patches.PawnCaching
{
	/// <summary>
	/// Clean pawns from PawnPathingCache when they are despawned.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
	internal static class Pawn_Despawn
	{
		internal static void Postfix(ref Pawn __instance)
		{
			PawnPathingCache.Remove(__instance);
		}
	}
}