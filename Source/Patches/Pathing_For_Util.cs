using Verse;
using Verse.AI;

namespace TerrainPathfindingKit.Patches
{
	/// <summary>
	/// Common implementation for prefixes to Pathing.For.
	/// </summary>
	public static class Pathing_For_Util
	{
		public static bool Prefix(Pathing instance, ref PathingContext result, Pawn pawn)
		{
			var terrainPathing = Getter.GetTerrainPathing(instance.Normal.map);
			if (terrainPathing == null)
			{
				return true;
			}

			var pathingType = terrainPathing.TypeFor(pawn);
			var context = terrainPathing.ContextFor(pathingType);
			if (context == null)
			{
				return true;
			}

			result = context;
			return false;
		}
	}
}