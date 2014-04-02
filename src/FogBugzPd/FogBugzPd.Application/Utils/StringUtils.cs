using System;

namespace FogBugzPd.Application.Utils
{
	public static class StringUtils
	{
		public static string FormatDecimal(decimal? lat, int afterPoint)
		{
			var format = "0.";
			for (var i = 0; i < afterPoint; i++)
			{
				format += "0";
			}
			return lat.HasValue ? lat.Value.ToString(format) : null;
		}

		public static string FormatSignDecimal(decimal? value, int afterPoint, int roundSign = -1)
		{
			if (value == null) return null;

			decimal fract = value.Value - (int) value.Value;

			if (fract == 0)
			{
				afterPoint = 0;
				roundSign = 0;
			}
			
			if (roundSign >0)
				value = Math.Round(value.Value, roundSign);

			var result = string.Empty;

			//if (value > 0) result = "+";

			return result + FormatDecimal(value, afterPoint);
		}

		public static string FormatNonSignDecimal(decimal? value, int afterPoint, int roundSign = 2)
		{
			if (value == null) return null;

			decimal fract = value.Value - (int)value.Value;

			if (fract == 0)
			{
				afterPoint = 0;
				roundSign = 0;
			}

			value = Math.Abs(Math.Round(value.Value, roundSign));

			return FormatDecimal(value, afterPoint);
		}

		public static string FormatDateTime(this DateTime? dateTime)
		{
			if (!dateTime.HasValue) return String.Empty;
			return dateTime.Value.FormatDateTime();
		}

		public static string FormatDateTime(this DateTime dateTime)
		{
			return FormatShortDate(dateTime);
		}

		public static string FormatShortDate(this DateTime? dateTime)
		{
			if (!dateTime.HasValue) return String.Empty;
			return dateTime.Value.FormatShortDate();
		}

		public static string FormatShortDate(this DateTime dateTime)
		{
			return dateTime.ToString("MMM dd, yyyy");
		}
	}
}


