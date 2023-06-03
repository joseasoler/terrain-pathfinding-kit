using System.Collections.Generic;
using Verse;

namespace TerrainPathfindingKit
{
	/// <summary>
	/// Keeps track of the pathing type that should be used for each pawn.
	/// </summary>
	public class PawnPathingType
	{
		/// <summary>
		/// This cache can be safely reused by all maps.
		/// </summary>
		private static readonly Dictionary<ThingDef, PathingType> ByThingDef =
			new Dictionary<ThingDef, PathingType>();
		
		public PathingType For(Pawn pawn)
		{
			if (pawn == null)
			{
				return PathingType.Default;
			}

			// ToDo: Hediff and item pathing context type changes.

			var thingDef = pawn.def;
			if (!ByThingDef.ContainsKey(thingDef))
			{
				var extension = thingDef.GetModExtension<PathingExtension>();
				ByThingDef[thingDef] = extension?.type ?? PathingType.Default;
			}

			return ByThingDef[thingDef];
		}
	}
}