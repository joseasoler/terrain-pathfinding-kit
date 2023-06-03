using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TerrainPathfindingKit.PathGrids;
using Verse;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Stores the TerrainPathGrid to use during a PathFinder.FindPath call.
	/// </summary>
	public static class PathFinder_CurrentGrid_Util
	{
		private static TerrainPathGrid _currentGrid;

		public static void SetCurrentGrid(Pawn pawn)
		{
			_currentGrid = null;
			if (pawn == null)
			{
				return;
			}

			var terrainPathing = pawn.Map.GetComponent<TerrainPathing>();
			var pathingType = terrainPathing.TypeFor(pawn);
			_currentGrid = terrainPathing.GridFor(pathingType);
		}

		public static int TerrainExtraDraftedPerceivedPathCost(TerrainDef terrainDef)
		{
			return _currentGrid?.ExtraDraftedPerceivedPathCost(terrainDef) ?? terrainDef.extraDraftedPerceivedPathCost;
		}

		public static int TerrainExtraNonDraftedPerceivedPathCost(TerrainDef terrainDef)
		{
			return _currentGrid?.ExtraNonDraftedPerceivedPathCost(terrainDef) ?? terrainDef.extraNonDraftedPerceivedPathCost;
		}
		
		public static List<CodeInstruction> ReplacePerceivedPathCosts(List<CodeInstruction> instructions, string label)
		{
			// Replace calls to extraDraftedPerceivedPathCost with custom terrain code.
			bool replacedExtraDraftedPerceivedPathCost = false;
			FieldInfo extraDraftedPerceivedPathCostField = AccessTools.Field(type: typeof(TerrainDef),
				name: nameof(TerrainDef.extraDraftedPerceivedPathCost));
			MethodInfo terrainExtraDraftedPerceivedPathCostMethod = AccessTools.Method(typeof(PathFinder_CurrentGrid_Util),
				nameof(TerrainExtraDraftedPerceivedPathCost));

			// Replace calls to extraNonDraftedPerceivedPathCost with custom terrain code.
			bool replacedExtraNonDraftedPerceivedPathCost = false;
			FieldInfo extraNonDraftedPerceivedPathCostField = AccessTools.Field(type: typeof(TerrainDef),
				name: nameof(TerrainDef.extraNonDraftedPerceivedPathCost));
			MethodInfo terrainExtraNonDraftedPerceivedPathCostMethod = AccessTools.Method(typeof(PathFinder_CurrentGrid_Util),
				nameof(TerrainExtraNonDraftedPerceivedPathCost));
	
			var newInstructions = new List<CodeInstruction>();
			foreach (var instruction in instructions)
			{
				if (instruction.operand as FieldInfo == extraDraftedPerceivedPathCostField)
				{
					replacedExtraDraftedPerceivedPathCost = true;
					newInstructions.Add(new CodeInstruction(OpCodes.Call, terrainExtraDraftedPerceivedPathCostMethod));
				}
				else if (instruction.operand as FieldInfo == extraNonDraftedPerceivedPathCostField)
				{
					replacedExtraNonDraftedPerceivedPathCost = true;
					newInstructions.Add(new CodeInstruction(OpCodes.Call, terrainExtraNonDraftedPerceivedPathCostMethod));
				}
				else
				{
					newInstructions.Add(instruction);
				}
			}
			
			if (!replacedExtraDraftedPerceivedPathCost ||
			    !replacedExtraNonDraftedPerceivedPathCost)
			{
				Logging.Error(
					$"{label} unsuccessful. replacedExtraDraftedPerceivedPathCost: {replacedExtraDraftedPerceivedPathCost}, replacedExtraNonDraftedPerceivedPathCost: {replacedExtraNonDraftedPerceivedPathCost}");
			}

			return newInstructions;
		}
	}
}