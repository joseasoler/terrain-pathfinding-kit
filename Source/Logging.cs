using System;
using System.Reflection;
using Verse;

namespace TerrainPathfindingKit
{
	public static class Logging
	{
		private static readonly Assembly Reference = typeof(Logging).Assembly;
		private static readonly string Name = Reference.GetName().Name;
		private static readonly Version Version = Reference.GetName().Version;
		private static readonly string Prefix = $"[{Name} v{Version}] ";

		public static string Prefixed(string original)
		{
			return Prefix + original;
		}

		public static void Notice(string message)
		{
			Log.Message(Prefixed(message));
		}

		public static void Error(string message)
		{
			Log.Error(Prefixed(message));
		}

		public static void ErrorOnce(string message)
		{
			Log.ErrorOnce(Prefixed(message), message.GetHashCode());
		}
	}
}