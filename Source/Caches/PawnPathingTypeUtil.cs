using System.Collections.Generic;
using Verse;

namespace TerrainPathfindingKit.Caches
{
	/// <summary>
	/// Calculates the pathing type to use for a pawn.
	/// </summary>
	public static class PawnPathingTypeUtil
	{
		/// <summary>
		/// This cache can be safely reused by all maps.
		/// </summary>
		private static readonly Dictionary<ThingDef, PathingType> RacesWithPathingExtension =
			new Dictionary<ThingDef, PathingType>();

		public static void Initialize()
		{
			var defList = DefDatabase<ThingDef>.AllDefsListForReading;
			for (int index = 0; index < defList.Count; ++index)
			{
				var thingDef = defList[index];
				if (thingDef.race != null)
				{
					var extension = thingDef.GetModExtension<PathingExtension>();
					RacesWithPathingExtension[thingDef] = extension?.type ?? PathingType.Default;
				}
			}
		}

		public static PathingType For(Pawn pawn)
		{
			if (pawn != null)
			{
				// Pawn race.
				var extension = pawn.def.GetModExtension<PathingExtension>();
				return extension?.type ?? PathingType.Default;
			}

			return PathingType.Default;
		}
	}
}