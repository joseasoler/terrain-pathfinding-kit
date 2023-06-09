using HarmonyLib;
using TerrainPathfindingKit.Caches;
using TerrainPathfindingKit.PathGrids;
using Verse;

namespace TerrainPathfindingKit.Mod
{
	public class TerrainPathfindingKit : Verse.Mod
	{
		private const string PackageId = "terrain.pathfinding.kit";

		public TerrainPathfindingKit(ModContentPack content) : base(content)
		{
			var harmonyInstance = new Harmony(PackageId);
			harmonyInstance.PatchAll();
			LongEventHandler.ExecuteWhenFinished(AquaticTerrainCost.Initialize);
			Logging.Notice("Initialized!");
		}
	}
}