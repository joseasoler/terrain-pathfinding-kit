using System.Collections.Generic;
using RimWorld;
using TerrainPathfindingKit.DefOfs;
using Verse;

namespace TerrainPathfindingKit.PathGrids
{
	// ToDo expose all hardcoded strings and values.
	public static class AquaticTerrainCost
	{
		private const int DeepCost = 0;
		private const int ShallowCost = 5;
		private const int MovingCost = 10;
		private const int MarshCost = 20;
		private const int BaseLandCost = 2000;

		// Any terrain with these tags can be traversed by water creatures.
		private static readonly HashSet<string> WaterTerrainTags = new HashSet<string> {"Water", "River"};

		private static readonly Dictionary<TerrainDef, int> WaterSwimCost = new Dictionary<TerrainDef, int>
		{
			{TerrainDefOf.WaterDeep, DeepCost},
			{TerrainDefOf.WaterOceanDeep, DeepCost},
			{TerrainDefOf.WaterMovingChestDeep, DeepCost},
			{TerrainDefOf.WaterShallow, ShallowCost},
			{TerrainDefOf.WaterOceanShallow, ShallowCost},
			{TerrainDefOf.WaterMovingShallow, MovingCost},
			{Terrains.Marsh, MarshCost}
		};

		public static Dictionary<TerrainDef, int> Cost = new Dictionary<TerrainDef, int>();

		public static int PerceivedPathCost(TerrainDef def, int originalCost)
		{
			return IsAquatic(def) ? 0 : originalCost + BaseLandCost;
		}

		public static bool IsAquatic(TerrainDef def)
		{
			var tags = def.tags;
			if (tags != null)
			{
				for (int tagIndex = 0; tagIndex < tags.Count; ++tagIndex)
				{
					var tag = tags[tagIndex];
					if (WaterTerrainTags.Contains(tag))
					{
						return true;
					}
				}
			}

			return false;
		}

		public static void Initialize()
		{
			var terrains = DefDatabase<TerrainDef>.AllDefsListForReading;
			for (int terrainIndex = 0; terrainIndex < terrains.Count; ++terrainIndex)
			{
				var terrainDef = terrains[terrainIndex];
				if (!IsAquatic(terrainDef))
				{
					Cost[terrainDef] = BaseLandCost + terrainDef.pathCost;
					continue;
				}

				if (WaterSwimCost.TryGetValue(terrainDef, out var value))
				{
					Cost[terrainDef] = value;
				}
				else
				{
					Cost[terrainDef] = ShallowCost;
				}
			}

			Logging.Debug($"Initialized {nameof(AquaticTerrainCost)}: {Cost.Count} terrains.");
		}
	}
}