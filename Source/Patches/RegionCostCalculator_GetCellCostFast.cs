using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Replace extraNonDraftedPerceivedPathCost and extraDraftedPerceivedPathCost when needed.
	/// Uses the PathFinder_CurrentGrid_Util set by the PathFinder.FindPath transpiler.
	/// </summary>
	[HarmonyPatch(typeof(RegionCostCalculator), "GetCellCostFast")]
	internal static class RegionCostCalculator_GetCellCostFast
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return PathFinder_CurrentGrid_Util.ReplacePerceivedPathCosts(instructions.ToList(),
				nameof(RegionCostCalculator_GetCellCostFast));
		}
	}
}