using System;
using FogLampz.Model;

namespace FogBugzPd.Core.FogBugzApi
{
	public static class FixForExtensions
	{
		public static bool IsShared(this FixFor m)
		{
			return !m.IndexProject.HasValue;
		}

		public static DateTime? EndDate(this FixFor m)
		{
			return m.Date;
		}
	}
}