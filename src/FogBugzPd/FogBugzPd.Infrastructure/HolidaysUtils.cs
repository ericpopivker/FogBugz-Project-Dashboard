using System;
using System.Collections.Generic;
using System.Linq;

namespace FogBugzPd.Infrastructure
{
	public static class HolidaysUtils
	{
		/// <summary>
		/// Gets sorted list of holidays up to year 2020
		/// <remarks>Please, someone, review the list and remove any unneeded holidays</remarks>
		/// </summary>
		/// <returns>Sorted list of holidays.</returns>
		public static IEnumerable<Holiday> GetHolidays()
		{
			yield return new Holiday(new DateTime(2012, 12, 24), "Christmas (day before)");
			yield return new Holiday(new DateTime(2012, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2012, 12, 31), "New Year's Eve");
			yield return new Holiday(new DateTime(2013, 1, 1), "New Year's Day");
			yield return new Holiday(new DateTime(2013, 2, 18), "President's Day");
			yield return new Holiday(new DateTime(2013, 5, 27), "Memorial Day");
			yield return new Holiday(new DateTime(2013, 7, 4), "Independence Day");
			yield return new Holiday(new DateTime(2013, 7, 5), "Independence Day (additional day)");
			yield return new Holiday(new DateTime(2013, 9, 2), "Labor Day");
			yield return new Holiday(new DateTime(2013, 11, 28), "Thanksgiving");
			yield return new Holiday(new DateTime(2013, 11, 29), "Thanksgiving (additional day)");
			yield return new Holiday(new DateTime(2013, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2014, 1, 1), "New Year's Day");
			yield return new Holiday(new DateTime(2014, 2, 17), "President's Day");
			yield return new Holiday(new DateTime(2014, 5, 26), "Memorial Day");
			yield return new Holiday(new DateTime(2014, 7, 4), "Independence Day");
			yield return new Holiday(new DateTime(2014, 9, 1), "Labor Day");
			yield return new Holiday(new DateTime(2014, 11, 27), "Thanksgiving");
			yield return new Holiday(new DateTime(2014, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2015, 1, 1), "New Year's Day");
			yield return new Holiday(new DateTime(2015, 2, 16), "President's Day");
			yield return new Holiday(new DateTime(2015, 5, 25), "Memorial Day");
			yield return new Holiday(new DateTime(2015, 7, 3), "Independence Day");
			yield return new Holiday(new DateTime(2015, 9, 7), "Labor Day");
			yield return new Holiday(new DateTime(2015, 11, 26), "Thanksgiving");
			yield return new Holiday(new DateTime(2015, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2016, 1, 1), "New Year's Day");
			yield return new Holiday(new DateTime(2016, 2, 15), "President's Day");
			yield return new Holiday(new DateTime(2016, 5, 30), "Memorial Day");
			yield return new Holiday(new DateTime(2016, 7, 4), "Independence Day");
			yield return new Holiday(new DateTime(2016, 9, 5), "Labor Day");
			yield return new Holiday(new DateTime(2016, 11, 24), "Thanksgiving");
			yield return new Holiday(new DateTime(2016, 12, 26), "Christmas");
			yield return new Holiday(new DateTime(2017, 1, 2), "New Year's Day");
			yield return new Holiday(new DateTime(2017, 2, 20), "President's Day");
			yield return new Holiday(new DateTime(2017, 5, 29), "Memorial Day");
			yield return new Holiday(new DateTime(2017, 7, 4), "Independence Day");
			yield return new Holiday(new DateTime(2017, 9, 4), "Labor Day");
			yield return new Holiday(new DateTime(2017, 11, 23), "Thanksgiving");
			yield return new Holiday(new DateTime(2017, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2018, 1, 1), "New Year's Day");
			yield return new Holiday(new DateTime(2018, 2, 19), "President's Day");
			yield return new Holiday(new DateTime(2018, 5, 28), "Memorial Day");
			yield return new Holiday(new DateTime(2018, 7, 4), "Independence Day");
			yield return new Holiday(new DateTime(2018, 9, 3), "Labor Day");
			yield return new Holiday(new DateTime(2018, 11, 22), "Thanksgiving");
			yield return new Holiday(new DateTime(2018, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2019, 1, 1), "New Year's Day");
			yield return new Holiday(new DateTime(2019, 2, 18), "President's Day");
			yield return new Holiday(new DateTime(2019, 5, 27), "Memorial Day");
			yield return new Holiday(new DateTime(2019, 7, 4), "Independence Day");
			yield return new Holiday(new DateTime(2019, 9, 2), "Labor Day");
			yield return new Holiday(new DateTime(2019, 11, 28), "Thanksgiving");
			yield return new Holiday(new DateTime(2019, 12, 25), "Christmas");
			yield return new Holiday(new DateTime(2020, 1, 1), "New Year's Day");
		}

		public static IList<Holiday> GetHolidaysInRange(DateTime firstDate, DateTime secondDate)
		{
			if (firstDate > secondDate)
				return new List<Holiday>(0);

			return GetHolidays().SkipWhile(holiday => holiday.Date < firstDate).TakeWhile(holiday => holiday.Date <= secondDate).ToList();
		}

		public static IList<DateTime> GetWeekendsInRange(DateTime firstDate, DateTime secondDate)
		{
			if (firstDate > secondDate)
				return new List<DateTime>(0);

			var list = new List<DateTime>();

			for (var dt = firstDate.AddDays(1); dt <= secondDate; dt = dt.AddDays(1))
			{
				if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
					list.Add(dt);
			}

			return list;
		}
	}

	public class Holiday
	{
		public DateTime Date { get; private set; }
		public string Description { get; private set; }

		public Holiday(DateTime date, string description)
		{
			Date = date;
			Description = description;
		}
	}
}
