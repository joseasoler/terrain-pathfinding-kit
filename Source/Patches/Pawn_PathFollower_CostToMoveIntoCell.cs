using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Use the correct costs for pawns using a TerrainPathGrid.
	/// </summary>
	[HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", typeof(Pawn), typeof(IntVec3))]
	internal static class Pawn_PathFollower_CostToMoveIntoCell
	{
		internal static int TerrainCalculatedCostAt(Map map, Pawn pawn, IntVec3 cell)
		{
			var terrainPathing = Getter.GetTerrainPathing(map);
			var prevCell = pawn.Position;

			var type = terrainPathing.TypeFor(pawn);
			var grid = terrainPathing.GridFor(type);
			return grid?.CostToMoveIntoCell(cell, prevCell) ??
			       map.pathing.For(pawn).pathGrid.CalculatedCostAt(cell, false, prevCell);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool ignoreInstructions = false;
			bool inserted = false;

			// Operand of the first line to be ignored. Insertion point for the new call.
			FieldInfo pathingField = AccessTools.Field(type: typeof(Map), name: nameof(Map.pathing));

			// Operand of the last line to be ignored.
			MethodInfo calculatedCostAtMethod = AccessTools.Method(typeof(PathGrid), nameof(PathGrid.CalculatedCostAt));

			// New call to be inserted.
			MethodInfo terrainCalculatedCostAtMethod = AccessTools.Method(typeof(Pawn_PathFollower_CostToMoveIntoCell),
				nameof(TerrainCalculatedCostAt));

			foreach (var instruction in instructions)
			{
				// Detect insertion point and start of the ignore interval.
				if (instruction.operand as FieldInfo == pathingField)
				{
					ignoreInstructions = true;
					inserted = true;
					yield return new CodeInstruction(OpCodes.Ldarg_0); // pawn.
					yield return new CodeInstruction(OpCodes.Ldarg_1); // cell.
					yield return new CodeInstruction(OpCodes.Call, terrainCalculatedCostAtMethod);
				}

				// Insert instructions unless they are being ignored.
				if (!ignoreInstructions)
				{
					yield return instruction;
				}

				// Detect end of the ignore interval.
				if (instruction.operand as MethodInfo == calculatedCostAtMethod)
				{
					ignoreInstructions = false;
				}
			}

			if (!inserted || ignoreInstructions)
			{
				Logging.Error(
					$"Pawn_PathFollower_CostToMoveIntoCell_Patch unsuccessful. inserted: {inserted}, ignoreInstructions: {ignoreInstructions}");
			}
		}
	}
}