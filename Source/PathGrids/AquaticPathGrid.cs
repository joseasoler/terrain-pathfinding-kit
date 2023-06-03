using TerrainPathfindingKit.CommonGrids;
using Verse;

namespace TerrainPathfindingKit.PathGrids
{
	public class AquaticPathGrid : TerrainPathGrid
	{
		public AquaticPathGrid(Map map, FireGrid fires, ThingsGrid things) : base(map, fires, things)
		{
		}

		public override int ExtraDraftedPerceivedPathCost(TerrainDef def)
		{
			return AquaticTerrainCost.IsAquatic(def) ? 0 : def.extraDraftedPerceivedPathCost;
		}

		public override int ExtraNonDraftedPerceivedPathCost(TerrainDef def)
		{
			return AquaticTerrainCost.IsAquatic(def) ? 0 : def.extraNonDraftedPerceivedPathCost;
		}

		protected override int TerrainCostAt(int cellIndex)
		{
			return AquaticTerrainCost.Cost[Map.terrainGrid.TerrainAt(cellIndex)];
		}
	}
}