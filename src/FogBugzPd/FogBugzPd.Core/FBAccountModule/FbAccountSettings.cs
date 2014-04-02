using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FogBugzPd.Core.FogBugzApi;
using FogLampz;
using FogLampz.Model;


namespace FogBugzPd.Core.FbAccountModule
{
	[Table("FbAccountSettings")]
	public class FbAccountSettings
	{
		#region Properties

		public int Id { get; set; }

		public int ResolvedVerifiedStatusId { get; set; }

		[Required]
		public bool AllowSubProjects { get; set; }

		//[Required]
		public int QaPercentage { get; set; }

		[Required]
		public bool AllowTestRail { get; set; }

		public virtual TestRailConfiguration TestRailConfig { get; set; }

		[NotMapped]
		public static Status DefaultStatus
		{
			get { return new Status {Index = 0, Name = "Closed"}; }
		}

		public string SubProjectTag { get; set; }

		public bool AllowQaEstimates { get; set; }

		public string QaEstimateCustomFieldName { get; set; }

		public bool SendDailyDigestEmails { get; set; }

		public string SendDailyDigestEmailsTo { get; set; }

		public Guid? Guid { get; set; }

		#endregion
		
		public FbAccountSettings()
		{
			ResolvedVerifiedStatusId = DefaultStatus.Index.Value;

			SubProjectTag = "SubProjectParentCase";

			AllowQaEstimates = false;
			QaPercentage = 30;
			QaEstimateCustomFieldName = "TestEstimate";

			AllowTestRail = false;

			SendDailyDigestEmails = false;
		}
	}
}
