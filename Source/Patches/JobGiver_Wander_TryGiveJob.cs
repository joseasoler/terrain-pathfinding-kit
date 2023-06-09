using HarmonyLib;
using RimWorld;
using TerrainPathfindingKit.Caches;
using TerrainPathfindingKit.DefOfs;
using TerrainPathfindingKit.PathGrids;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Inject a new job responsible for seeking safe terrain.
	/// </summary>
	[HarmonyPatch(typeof(JobGiver_Wander), "TryGiveJob")]
	internal static class JobGiver_Wander_TryGiveJob
	{
		internal static void Postfix(ref Job __result, Pawn pawn)
		{
			if (__result != null && __result.def == JobDefOf.Wait_Wander)
			{
				var grid = PawnPathingCache.GridFor(pawn);
				if (grid != null && !grid.CanEnterCell(pawn.Position))
				{
					Region region = ClosestRegionWithSafeTerrain(pawn, grid, out IntVec3 targetCell);
					if (region != null)
					{
						// ToDo this job is causing NREs in RegionCostCalculator.GetRegionDistance. Investigate and enable. 
						// __result = JobMaker.MakeJob(Jobs.TPK_GotoSafeTerrain, targetCell);
					}
				}
			}
		}

		private static Region ClosestRegionWithSafeTerrain(Pawn pawn, TerrainPathGrid grid, out IntVec3 targetCell)
		{
			targetCell = IntVec3.Invalid;
			IntVec3 root = pawn.Position;
			Region region = root.GetRegion(pawn.Map);
			if (region == null)
			{
				return null;
			}

			bool RegionEntryCondition(Region from, Region r)
			{
				return r.Allows(TraverseParms.For(pawn), false);
			}

			Region foundReg = null;
			IntVec3 foundCell = IntVec3.Invalid;

			bool RegionProcessor(Region r)
			{
				foreach (var cell in r.Cells)
				{
					if (grid.CanEnterCell(cell))
					{
						foundCell = cell;
						foundReg = r;
						return true;
					}
				}

				return false;
			}

			RegionTraverser.BreadthFirstTraverse(region, RegionEntryCondition, RegionProcessor, 9999);

			targetCell = foundCell;
			return foundReg;
		}
	}
}