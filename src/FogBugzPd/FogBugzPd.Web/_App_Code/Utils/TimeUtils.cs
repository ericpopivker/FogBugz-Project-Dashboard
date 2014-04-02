using System;

namespace FogBugzPd.Web.Utils
{
	public static class TimeUtils
	{
		public static DateTime Min(DateTime dt1, DateTime dt2)
		{
			return dt1 < dt2 ? dt1 : dt2;
		}

		public static DateTime Max(DateTime dt1, DateTime dt2)
		{
			return dt1 < dt2 ? dt2 : dt1;
		}

		public static long UnixTime(this DateTime dt)
		{
			return (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}
	}
}