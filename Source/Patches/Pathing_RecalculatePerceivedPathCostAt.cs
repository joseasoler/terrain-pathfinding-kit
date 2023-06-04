using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Recalculate terrain path grid cells after vanilla ones. Reuse the same haveNotified variable.
	/// </summary>
	[HarmonyPatch(typeof(Pathing), nameof(Pathing.RecalculatePerceivedPathCostAt))]
	internal static class Pathing_RecalculatePerceivedPathCostAt
	{
		internal static void TerrainRecalculatePerceivedPathCostAt(Pathing instance, IntVec3 c, ref bool haveNotified)
		{
			Getter.GetTerrainPathing(instance.Normal.map)?.RecalculatePerceivedPathCostAt(c, ref haveNotified);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo terrainRecalculatePerceivedPathCostAtMethod =
				AccessTools.Method(typeof(Pathing_RecalculatePerceivedPathCostAt),
					nameof(TerrainRecalculatePerceivedPathCostAt));

			object haveNotifiedOperand = null;

			foreach (var instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldloca_S)
				{
					// Store the haveNotified operand used by other recalculation calls.
					haveNotifiedOperand = instruction.operand;
				}

				if (instruction.opcode == OpCodes.Ret)
				{
					// Insert the new recalculation right before the method ends.
					yield return new CodeInstruction(OpCodes.Ldarg_0); // Pathing instance
					yield return new CodeInstruction(OpCodes.Ldarg_1); // Cell
					yield return new CodeInstruction(OpCodes.Ldloca_S, haveNotifiedOperand); // haveNotified
					yield return new CodeInstruction(OpCodes.Call, terrainRecalculatePerceivedPathCostAtMethod);
				}

				yield return instruction;
			}

			if (haveNotifiedOperand == null)
			{
				Logging.Error("Pathing_RecalculatePerceivedPathCostAt_Patch unsuccessful. Could not find haveNotifiedOperand");
			}
		}
	}
}