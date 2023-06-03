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
		public static ByteGrid TerrainAvoidGrid(Map map, Pawn pawn)
		{
			var terrainPathing = Getter.GetTerrainPathing(map);
			var pathingType = terrainPathing.TypeFor(pawn);
			var grid = terrainPathing.GridFor(pathingType);
			return grid != null ? grid.AvoidGrid(map.avoidGrid.Grid) : map.avoidGrid.Grid;
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			// Set the current path grid at the beginning of the execution.
			bool insertedSetCurrentGrid = false;
			MethodInfo setCurrentGridMethod = AccessTools.Method(typeof(PathFinder_CurrentGrid_Util),
				nameof(PathFinder_CurrentGrid_Util.SetCurrentGrid));

			// Replace the avoid grid with our custom one.
			// This requires ignoring two lines in the original code.
			bool replacedAvoidGrid = false;
			FieldInfo avoidGridField = AccessTools.Field(typeof(Map), nameof(Map.avoidGrid));
			MethodInfo terrainAvoidGridMethod =
				AccessTools.Method(typeof(PathFinder_FindPath), nameof(TerrainAvoidGrid));

			var instructionList =
				PathFinder_CurrentGrid_Util.ReplacePerceivedPathCosts(instructions.ToList(), nameof(PathFinder_FindPath));

			bool ignoreNext = false;
			foreach (var instruction in instructionList)
			{
				if (instruction.opcode == OpCodes.Ldfld && instruction.operand as FieldInfo == avoidGridField)
				{
					// The previous line already loaded the map. Load the pawn and our custom call.
					yield return new CodeInstruction(OpCodes.Ldloc_0); // Pawn
					yield return new CodeInstruction(OpCodes.Call, terrainAvoidGridMethod);
					// Replacing the avoid grid field requires ignoring this line and the next.
					ignoreNext = true;
					replacedAvoidGrid = true;
				}
				else
				{
					if (ignoreNext)
					{
						ignoreNext = false;
					}
					else
					{
						yield return instruction;
					}
				}

				if (instruction.opcode == OpCodes.Stloc_0)
				{
					// Insert setCurrentGridMethod call after the pawn variable is initialized.
					yield return new CodeInstruction(OpCodes.Ldloc_0); // Pawn
					yield return new CodeInstruction(OpCodes.Call, setCurrentGridMethod);
					insertedSetCurrentGrid = true;
				}
			}

			if (!insertedSetCurrentGrid || !replacedAvoidGrid)
			{
				Logging.Error(
					$"PathFinder_FindPath_Patch unsuccessful. insertedSetCurrentGrid: {insertedSetCurrentGrid}, replacedAvoidGrid: {replacedAvoidGrid}");
			}
		}
	}
}