using System.Collections.Generic;
using Verse;

namespace TerrainPathfindingKit.Caches
{
	/// <summary>
	/// Global terrain pathing cache. Updated after map generation, game loading and map removal.
	/// </summary>
	public static class TerrainPathingCache
	{
		/// <summary>
		/// The key is the Map's uniqueID, which is guaranteed to be unique by UniqueIDsManager.
		/// </summary>
		private static readonly Dictionary<int, TerrainPathing> TerrainPathingSet = new Dictionary<int, TerrainPathing>();

		public static void Add(Map map, TerrainPathing component)
		{
			TerrainPathingSet[map.uniqueID] = component;
		}

		public static void Remove(Map map)
		{
			TerrainPathingSet.Remove(map.uniqueID);
		}

		public static TerrainPathing Get(Map map)
		{
			return TerrainPathingSet.TryGetValue(map.uniqueID, out TerrainPathing value) ? value : null;
		}
	}
}