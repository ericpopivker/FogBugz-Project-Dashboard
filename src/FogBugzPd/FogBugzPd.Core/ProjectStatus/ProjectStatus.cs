using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Core.ProjectStatus
{
	public enum FogbugzProjectStatus
	{
		[StringValue("Very Behind")]
		VeryBehind = 1,

		[StringValue("Little Behind")]
		LittleBehind,

		[StringValue("On Time")]
		OnTime,

		[StringValue("A Little Ahead")]
		ALittleAhead,

		[StringValue("Very Ahead")]
		VeryAhead,

		[StringValue("Not Started")]
		NotStarted
	}
	
	public class ProjectStatus
	{
		public int ProjectId { get; set; }

		public int MilestoneId { get; set; }

		public List<EmployeeStatus> EmployeeStatuses { get; set; }

		public decimal TotalRemainingDays { get; set; }

		public decimal TotalOverUnder { get; set; }

		public decimal TotalWorkedDays { get; set; }

		public decimal TotalPercentageWorkedDays { get; set; }

		public decimal TotalAdjustedOverUnder { get; set; }

		public decimal TotalActualOverUnder { get; set; }

		public decimal TotalScheduledWorkDays { get; set; }

		public int DaysToRelease { get; set; }

		public decimal? StatusValue { get; set; }

		public FogbugzProjectStatus Status { get; set; }

		public string OverUnderLabel(decimal value)
		{

			if (value < 0) return "over";
			if (value > 0) return "under";

			return "";
		}

		public decimal EstimatedDays
		{
			get { return XConvert.RoundDecimalToHalf(DaysToRelease - TotalRemainingDays); }
		}

		public ProjectStatus()
		{
			EmployeeStatuses = new List<EmployeeStatus>();
		}
	}
}
