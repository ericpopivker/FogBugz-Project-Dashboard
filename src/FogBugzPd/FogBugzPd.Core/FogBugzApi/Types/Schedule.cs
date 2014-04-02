using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class TimeOffRange
	{
		public string Name { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
	}

	public class Schedule
	{
		public List<TimeOffRange> TimeOffRanges;

		public Schedule()
		{
			TimeOffRanges = new List<TimeOffRange>();
		}
	}

	public class ScheduledDaysResult
	{
		public int ScheduleWorkDaysCount { get; set; }
		public int DaysOffCount { get; set; }
		public List<TimeOffRange> TimeOffRanges { get; set; }

		public ScheduledDaysResult()
		{
			TimeOffRanges = new List<TimeOffRange>();
		}
	}
	
	public class FbSchedule
	{
		public Schedule SiteSchedule { get; set; }

		public Dictionary<int, Schedule> PersonSchedules { get; set; }

		public FbSchedule()
		{
			SiteSchedule = new Schedule();
			PersonSchedules = new Dictionary<int, Schedule>();
		}

		public ScheduledDaysResult GetSiteScheduledDays(DateTime fromDate, DateTime toDate)
		{
			return GetScheduledDaysResult(this.SiteSchedule.TimeOffRanges, fromDate, toDate);
		}

		public ScheduledDaysResult GetScheduledDays(int personId, DateTime fromDate, DateTime toDate)
		{
			Schedule personSchedule;

			PersonSchedules.TryGetValue(personId, out personSchedule);

			if (personSchedule == null)
				return GetSiteScheduledDays(fromDate, toDate);

			return GetScheduledDaysResult(personSchedule.TimeOffRanges, fromDate, toDate);
		}

		private ScheduledDaysResult GetScheduledDaysResult(List<TimeOffRange> timeOffRanges, DateTime fromDate, DateTime toDate)
		{
			var scheduledDaysResult = new ScheduledDaysResult();

			var daysLeftWithWeekends = Convert.ToInt32((toDate - fromDate).TotalDays);
			var weekendsCount = HolidaysUtils.GetWeekendsInRange(fromDate, toDate).Count;

			scheduledDaysResult.ScheduleWorkDaysCount = daysLeftWithWeekends - weekendsCount;

			foreach (var timeOffRange in timeOffRanges)
			{
				var fromDatesCompare = DateTime.Compare(timeOffRange.FromDate, fromDate);
				var toDatesCompare = DateTime.Compare(timeOffRange.ToDate, toDate);
				var checkBeginOfHoliday = DateTime.Compare(timeOffRange.FromDate, toDate);
				var checkEndOfHoliday = DateTime.Compare(timeOffRange.ToDate, fromDate);

				if ((fromDatesCompare >= 0 && toDatesCompare <= 0) ||
					(fromDatesCompare < 0 && toDatesCompare < 0 && checkEndOfHoliday > 0) ||
					(fromDatesCompare > 0 && toDatesCompare > 0 && checkBeginOfHoliday < 0))
				{
					scheduledDaysResult.TimeOffRanges.Add(timeOffRange);
				}
			}

			var daysOffCount = 0;

			if (scheduledDaysResult.TimeOffRanges.Any())
			{
				foreach (var timeOffRange in scheduledDaysResult.TimeOffRanges)
				{
					var fromDatesCompare = DateTime.Compare(timeOffRange.FromDate, fromDate);
					var toDatesCompare = DateTime.Compare(timeOffRange.ToDate, toDate);
					var checkBeginOfHoliday = DateTime.Compare(timeOffRange.FromDate, toDate);
					var checkEndOfHoliday = DateTime.Compare(timeOffRange.ToDate, fromDate);

					if (fromDatesCompare >= 0 && toDatesCompare <= 0)
					{
						weekendsCount = HolidaysUtils.GetWeekendsInRange(timeOffRange.FromDate, timeOffRange.ToDate).Count;
						
						daysOffCount += Convert.ToInt32((timeOffRange.ToDate - timeOffRange.FromDate).TotalDays) - weekendsCount;
					}
					if (fromDatesCompare > 0 && toDatesCompare > 0 && checkBeginOfHoliday < 0)
					{
						weekendsCount = HolidaysUtils.GetWeekendsInRange(timeOffRange.FromDate, toDate).Count;
						daysOffCount += Convert.ToInt32((toDate - timeOffRange.FromDate).TotalDays) - weekendsCount;
					}
					if (fromDatesCompare < 0 && toDatesCompare < 0 && checkEndOfHoliday > 0)
					{
						weekendsCount = HolidaysUtils.GetWeekendsInRange(fromDate, timeOffRange.ToDate).Count;
						daysOffCount += Convert.ToInt32((timeOffRange.ToDate - fromDate).TotalDays) - weekendsCount;
					}
				}
			}

			scheduledDaysResult.DaysOffCount = daysOffCount;

			return scheduledDaysResult;
		}
	}
}
