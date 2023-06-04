using System.Collections.Generic;
using Verse;

namespace TerrainPathfindingKit
{
	/// <summary>
	/// Utility class for getting stuff quickly.
	/// </summary>
	public static class Getter
	{
		private static readonly Dictionary<int, TerrainPathing> _terrainPathings = new Dictionary<int, TerrainPathing>();

		public static void AddMap(Map map, TerrainPathing component)
		{
			_terrainPathings[map.uniqueID] = component;
		}

		public static void RemoveMap(Map map)
		{
			_terrainPathings.Remove(map.uniqueID);
		}

		public static TerrainPathing GetTerrainPathing(Map map)
		{
			return _terrainPathings.TryGetValue(map.uniqueID, out TerrainPathing value) ? value : null;
		}
	}
}