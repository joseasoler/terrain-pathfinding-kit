using TerrainPathfindingKit.Caches;
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
			PathingContext context = null;
			if (pawn != null)
			{
				context = PawnPathingCache.ContextFor(pawn);

				if (context != null)
				{
					result = context;
				}
			}

			return context == null;
		}
	}
}