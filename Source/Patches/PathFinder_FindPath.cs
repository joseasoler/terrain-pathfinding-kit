using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Initialize PathFinder_CurrentGrid_Util.CurrentGrid for this execution of PathFinder.FindPath.
	/// Replace extraNonDraftedPerceivedPathCost and extraDraftedPerceivedPathCost when needed.
	/// </summary>
	[HarmonyPatch(typeof(PathFinder), nameof(PathFinder.FindPath), typeof(IntVec3), typeof(LocalTargetInfo),
		typeof(TraverseParms), typeof(PathEndMode), typeof(PathFinderCostTuning))]
	internal static class PathFinder_FindPath
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			// Set the current path grid at the beginning of the execution.
			bool insertedSetCurrentGrid = false;
			MethodInfo setCurrentGridMethod = AccessTools.Method(typeof(PathFinder_CurrentGrid_Util),
				nameof(PathFinder_CurrentGrid_Util.SetCurrentGrid));

			var instructionList =
				PathFinder_CurrentGrid_Util.ReplacePerceivedPathCosts(instructions.ToList(), nameof(PathFinder_FindPath));

			foreach (var instruction in instructionList)
			{
				yield return instruction;

				if (instruction.opcode == OpCodes.Stloc_0)
				{
					// Insert setCurrentGridMethod call after the pawn variable is initialized.
					yield return new CodeInstruction(OpCodes.Ldloc_0); // Pawn
					yield return new CodeInstruction(OpCodes.Call, setCurrentGridMethod);
					insertedSetCurrentGrid = true;
				}
			}

			if (!insertedSetCurrentGrid)
			{
				Logging.Error("PathFinder_FindPath_Patch unsuccessful. Setting the current grid has not been patched.");
			}
		}
	}
}