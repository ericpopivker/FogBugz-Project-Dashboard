using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FogBugzPd.Infrastructure
{
	public static class DateUtils
	{
		public static int? Difference(DateTime firstDate, DateTime secondDate, bool countWeekendsAndHolidays = true)
		{
			if (firstDate == secondDate) return 0;

			int daysOffCount = 0;

			if (countWeekendsAndHolidays)
			{
				var holidaysInRange = HolidaysUtils.GetHolidaysInRange(firstDate, secondDate);
				var weekendsInRange = HolidaysUtils.GetWeekendsInRange(firstDate, secondDate);

				var combinedDates = holidaysInRange.Select(holiday => holiday.Date).Union(weekendsInRange).Distinct();

				daysOffCount = combinedDates.Count();
			}

			var retVal = (secondDate - firstDate).TotalDays;

			if (retVal > 0) retVal -= daysOffCount;
			else retVal += daysOffCount;

			return (int) retVal;
		}

		public static DateTime? GetCodeFreezeFromName(string name, Regex regEx)
		{
			Match match = regEx.Match(name);

			int day, month, year;

			if (match.Success &&
				int.TryParse(match.Groups["day"].Value, out day) &&
				int.TryParse(match.Groups["month"].Value, out month) &&
				int.TryParse(match.Groups["year"].Value, out year))
			{
				if (year < 100) year += 2000;

				return new DateTime(year, month, day);
			}

			return null;
		}
	}
}
