using System;

namespace FogBugzPd.Web.Utils
{
	public static class StringUtils
	{
		public static string FormatDateTime(this DateTime? dateTime)
		{
			if (!dateTime.HasValue) return String.Empty;
			return dateTime.Value.FormatDateTime();
		}

		public static string FormatTime(DateTime? dateTime)
		{
			if (dateTime == null)
				return "";

			string result = dateTime.Value.ToString("h\\:mmtt");
			return result.ToLower();
		}

		public static string FormatDateTime(this DateTime dateTime)
		{
			return FormatShortDate(dateTime) + "  " + FormatTime(dateTime);
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

		public static string FormatDuration(this int durationHours)
		{
			return Convert.ToDecimal(durationHours).FormatDuration();
		}

		public static string FormatDurationMin(this int durationMins)
		{
			return (Convert.ToDecimal(durationMins)/60).FormatDuration();
		}

		public static string FormatDuration(this decimal hours)
		{
			if (hours == 0) return "0 hours";

			if (hours < 8)
			{
				return String.Format("{0:0.#} hours", Math.Round(hours, 1, MidpointRounding.AwayFromZero));
			}

			return String.Format("{0:0.#} days", Math.Round(hours/8, 1, MidpointRounding.AwayFromZero));
		}

		public static string FormatPercent(int percent)
		{
			return FormatPercent((double)percent);
		}

		public static string FormatPercent(double? percent)
		{
			return FormatPercent((decimal?)percent);
		}

		public static string FormatPercent(decimal? percent)
		{
			return FormatPercent(percent, 0);
		}

		public static string FormatPercent(decimal? percent, int decimals)
		{
			if (percent == null)
				return "";

			percent = Decimal.Round(percent.Value, decimals);

			string result = percent + "%";
			return result;
		}

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
	}
}


