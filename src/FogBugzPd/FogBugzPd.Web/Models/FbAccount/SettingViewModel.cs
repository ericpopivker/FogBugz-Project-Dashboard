using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FogBugzPd.Core.FogBugzApi;
using System.Linq;
using FogLampz.Model;

namespace FogBugzPd.Web.Models.FbAccount
{
	public class SettingViewModel
	{
		[Required]
		public int Id { get; set; }

		[Display(Name = "Resolved (Verified) Status")]
		public int ResolvedVerifiedStatusId { get; set; }

		[Display(Name = "Use Sub-Projects")]
		public bool AllowSubProjects { get; set; }

		[Display(Name = "QA Percentage")]
		[CustomValidation.QaPercentage(ErrorMessage = "Please fill QA percentage correctly.")]
		public int? QaPercentage { get; set; }

		[Display(Name = "Integrate with Test Rail")]
		public bool AllowTestRail { get; set; }

		[Display(Name = "Url")]
		[CustomValidation.GenericUrl]
		[CustomValidation.TestRailConfig(ErrorMessage = "Url is required")]
		public string TestRailUrl { get; set; }

		[Display(Name = "Token")]
		[CustomValidation.TestRailConfig(ErrorMessage = "Token is required")]
		[MaxLength(32)]
		public string TestRailToken { get; set; }


		[Display(Name = "Send daily status report")]
		public bool AllowSendDailyDigestEmails { get; set; }

		[Display(Name = "Email")]
		[CustomValidation.EmailsList(ErrorMessage = "Enter valid emails comma separated")]
		public string SendDailyDigestEmailsTo { get; set; }

		public SelectList StatusesAvailable { get; set; }

		public void Setup()
		{
			var statuses = FogBugzGateway.GetStatuses()
				.ToList();

			statuses.Add(Core.FbAccountModule.FbAccountSettings.DefaultStatus);

			statuses = statuses.Distinct(new StatusComparer()).ToList();
			StatusesAvailable = new SelectList(statuses, "Index", "Name");
		}

		[CustomValidation.AllowParentCaseAttribute(ErrorMessage = "Field is required")]
		[Display(Name = "Sub-Project Tag")]
		public string SubProjectTag { get; set; }

		[Display(Name = "Use QA Estimates")]
		public bool AllowQaEstimates { get; set; }

		[Display(Name = "QA Estimate Custom Field Name")]
		[CustomValidation.QaCustomField(ErrorMessage = "Field name is required")]
		public string QaEstimateCustomFieldname { get; set; }

	}

	class StatusComparer : IEqualityComparer<Status>
	{
		public bool Equals(Status x, Status y)
		{
			return x.Name == y.Name;
		}

		public int GetHashCode(Status status)
		{
			return (status.Name).GetHashCode();
		}

	}
}