using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TerrainPathfindingKit
{
	/// <summary>
	/// This extension makes pawns use a custom pathing type. The following Defs are supported. In order of priority:
	/// * HediffDef (pawns that currently have this Hediff).
	/// * ThingDef (pawns wearing this apparel).
	/// * TraitDef (pawns having this trait).
	/// * LifeStageDef (pawns currently in this life stage).
	/// * ThingDef (pawns with this ThingDef as their race).
	///
	/// Currently, a pawn with PathingExtension from multiple sources will only use the one with highest priority.
	/// ToDo: The rules for this might change during development.
	/// </summary>
	public class PathingExtension : DefModExtension
	{
		public PathingType type;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var line in base.ConfigErrors())
			{
				yield return line;
			}

			if (type == PathingType.Default || type == PathingType.Count)
			{
				yield return Logging.Prefixed(
					$"PathingExtension: Using {Enum.GetName(typeof(PathingType), type)} as a type is not allowed.");
			}
		}
	}
}