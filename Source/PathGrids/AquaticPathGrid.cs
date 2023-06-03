using TerrainPathfindingKit.CommonGrids;
using Verse;

namespace TerrainPathfindingKit.PathGrids
{
	public class AquaticPathGrid : TerrainPathGrid
	{
		private ByteGrid _aquaticAvoidGrid;
		private ByteGrid _combinedAvoidGrid;

		public AquaticPathGrid(Map map, FireGrid fires, ThingsGrid things) : base(map, fires, things)
		{
			_aquaticAvoidGrid = new ByteGrid(map);
			_combinedAvoidGrid = new ByteGrid(map);
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

		public override ByteGrid AvoidGrid(ByteGrid defaultGrid)
		{
			return defaultGrid != null ? _combinedAvoidGrid : _aquaticAvoidGrid;
		}

		public override void UpdateAvoidGridCell(int cellIndex)
		{
			var terrainDef = Map.terrainGrid.TerrainAt(cellIndex);
			if (AquaticTerrainCost.IsAquatic(terrainDef))
			{
				_aquaticAvoidGrid[cellIndex] = byte.MinValue;
				_combinedAvoidGrid[cellIndex] = Map.avoidGrid.Grid[cellIndex];
			}
			else
			{
				_aquaticAvoidGrid[cellIndex] = byte.MaxValue;
				_combinedAvoidGrid[cellIndex] = byte.MaxValue;
			}
		}

		public override void RegenerateAvoidGrid()
		{
			for (int cellIndex = 0; cellIndex < _combinedAvoidGrid.CellsCount; ++cellIndex)
			{
				UpdateAvoidGridCell(cellIndex);
			}
		}
	}
}