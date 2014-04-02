using System;

namespace FogBugzPd.Core.ProjectStatus
{
	public class EmployeeStatus
	{
		public string Username { get; set; }

		public decimal RemainingDays { get; set; }

		public decimal OverUnder { get; set; }

		public int WorkedDays { get; set; }

		public decimal PercentageWorkedDays { get; set; }

		public decimal AdjustedOverUnder { get; set; }

		public decimal ActualOverUnder { get; set; }

		public DateTime MilestoneStartDate { get; set; }

		public int BusinessDays { get; set; }

		public DateTime? FirstWorkedDay { get; set; }

		public decimal ScheduledWorkDays { get; set; }
	}
}
