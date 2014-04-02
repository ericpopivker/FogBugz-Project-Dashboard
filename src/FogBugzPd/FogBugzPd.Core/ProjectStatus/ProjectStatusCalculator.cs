using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Utils;
using FogLampz.Model;

namespace FogBugzPd.Core.ProjectStatus
{
	public class ProjectStatusCalculator
	{
		private CaseSet _caseSet;
		private List<Person> _persons;
		private bool _isScheduleLoadedByAdmin;
		private FbSchedule _fbSchedule;

		public ProjectStatusCalculator(CaseSet caseSet, List<Person> persons, bool isLoadedByAdmin, FbSchedule schedule)
		{
			_caseSet = caseSet;

			_persons = persons;

			_isScheduleLoadedByAdmin = isLoadedByAdmin;

			_fbSchedule = schedule;
		}

		public ProjectStatus GetProjectStatus()
		{
			var result = new ProjectStatus();

			var closedPerson = _persons.SingleOrDefault(p => p.Name == "*Closed");

			var usersIds = _caseSet.Cases
				.Where(c => c.IndexPersonAssignedTo != null)
				.Select(c => c.IndexPersonAssignedTo)
				.Distinct().ToList();

			if (closedPerson != null) usersIds = usersIds.Where(u => u.Value != closedPerson.Index.Value).ToList();

			var projectStartDate = _caseSet.Milestone.DateStart.HasValue ? _caseSet.Milestone.DateStart.Value : DateTime.Now.Date;

			result.DaysToRelease = DateUtils.Difference(DateTime.Now, _caseSet.Milestone.DateRelease.Value).Value;

			var cfRegex = new Regex(@"\(CF *(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2}(?:\d{2})?)\)", RegexOptions.Singleline | RegexOptions.Compiled);

			DateTime? endDate = DateUtils.GetCodeFreezeFromName(_caseSet.Milestone.Name, cfRegex) ??
								_caseSet.Milestone.DateRelease;

			result.EmployeeStatuses.AddRange(usersIds.Select(u => CalculateEmployeeStatus(u.Value, projectStartDate, endDate)).Where(r => r != null).ToList());

			if (result.EmployeeStatuses.Count != 0)
			{
				result.TotalRemainingDays = XConvert.RoundDecimalToHalf(result.EmployeeStatuses.Sum(es => es.RemainingDays) / result.EmployeeStatuses.Count);
				result.TotalOverUnder = XConvert.RoundDecimalToHalf(result.EmployeeStatuses.Sum(es => es.OverUnder));
				result.TotalWorkedDays = (decimal)result.EmployeeStatuses.Sum(es => es.WorkedDays) / result.EmployeeStatuses.Count;
				result.TotalPercentageWorkedDays = result.EmployeeStatuses.Sum(es => es.PercentageWorkedDays) / result.EmployeeStatuses.Count;
				result.TotalAdjustedOverUnder = XConvert.RoundDecimalToHalf(result.EmployeeStatuses.Sum(es => es.AdjustedOverUnder));
				result.TotalActualOverUnder = XConvert.RoundDecimalToHalf(result.EmployeeStatuses.Sum(es => es.ActualOverUnder));
				result.TotalScheduledWorkDays = XConvert.RoundDecimalToHalf(result.EmployeeStatuses.Sum(es => es.ScheduledWorkDays) / result.EmployeeStatuses.Count);

				result.StatusValue = XConvert.RoundDecimalToHalf(result.TotalActualOverUnder / result.EmployeeStatuses.Count);
			}

			result.Status = GetProjectStatus(result.StatusValue);

			return result;
		}

		private EmployeeStatus CalculateEmployeeStatus(int personId, DateTime startDate, DateTime? endDate)
		{
			var person = _persons.SingleOrDefault(p => p.Index == personId);

			if (person != null)
			{
				var result = new EmployeeStatus() { Username = person.Name };

				var cases = _caseSet.Cases.Where(c => c.IndexPersonAssignedTo == person.Index).ToList();

				//calculate number of worked days
				var updatedCasesByUser =
					cases.Where(c => c.IndexPersonResolvedBy.HasValue && c.IndexPersonResolvedBy.Value == personId && c.DateResolved.HasValue).ToList();

				if (updatedCasesByUser.Count > 0)
				{
					var firstWorkedDay = updatedCasesByUser.Select(c => c.DateResolved.Value).Min().Date;

					result.FirstWorkedDay = firstWorkedDay;

					result.WorkedDays = Math.Abs(DateUtils.Difference(DateTime.Now, firstWorkedDay).Value);
				}

				//calculate percent of worked days
				//var elapsedTime = cases.Sum(@case => @case.HoursElapsed.GetValueOrDefault(0)) / 8m;

				result.MilestoneStartDate = startDate;
				result.BusinessDays = Math.Abs(DateUtils.Difference(DateTime.Now, startDate).Value);

				//result.ScheduledWorkDays = Math.Abs(DateUtils.Difference(DateTime.Now, endDate.Value).Value);

				if (_isScheduleLoadedByAdmin)
				{
					var scheduledDays = _fbSchedule.GetScheduledDays(personId, DateTime.Now, endDate.Value);
					result.ScheduledWorkDays = scheduledDays.ScheduleWorkDaysCount - scheduledDays.DaysOffCount;
				}
				else
				{
					var scheduledDays = _fbSchedule.GetSiteScheduledDays(DateTime.Now, endDate.Value);
					result.ScheduledWorkDays = scheduledDays.ScheduleWorkDaysCount - scheduledDays.DaysOffCount;
				}
				

				result.PercentageWorkedDays = result.WorkedDays != 0 ? (result.WorkedDays * 100 / (result.BusinessDays == 0 ? 1 : result.BusinessDays)) : 50;

				//calculate remaining days
				decimal remainingTime = 0;
				//foreach (var activeCase in cases.Where(CaseUtils.IsActive))
				foreach (var activeCase in cases.Where(c => c.IsOpen))
				{
					var remaining = 0m;
					var estimate = activeCase.HoursCurrentEstimate ?? activeCase.HoursOriginalEstimate;
					if (estimate.HasValue)
					{
						remaining = estimate.Value - (activeCase.HoursElapsed ?? 0);
						if (remaining < 0) remaining = 0;
					}
					remainingTime += remaining;
				}

				result.RemainingDays = XConvert.RoundDecimalToHalf(remainingTime / 8);

				if (result.RemainingDays == 0) return null;

				//calculateOver/Under
				result.OverUnder = XConvert.RoundDecimalToHalf(result.ScheduledWorkDays - result.RemainingDays);

				result.AdjustedOverUnder = XConvert.RoundDecimalToHalf(result.OverUnder < 0
											   ? result.OverUnder / (result.PercentageWorkedDays / 100)
											   : result.OverUnder * (result.PercentageWorkedDays / 100));

				result.ActualOverUnder = XConvert.RoundDecimalToHalf(result.OverUnder < 0 ? result.OverUnder * 2 : result.OverUnder);

				return result;
			}

			return null;
		}

		private FogbugzProjectStatus GetProjectStatus(decimal? status)
		{
			if (!status.HasValue) return FogbugzProjectStatus.NotStarted;

			if (status < -3) return FogbugzProjectStatus.VeryBehind;
			if (status <= 0) return FogbugzProjectStatus.LittleBehind;
			if (status < 3) return FogbugzProjectStatus.OnTime;
			if (status < 5) return FogbugzProjectStatus.ALittleAhead;
			return FogbugzProjectStatus.VeryAhead;
		}
	}
}
