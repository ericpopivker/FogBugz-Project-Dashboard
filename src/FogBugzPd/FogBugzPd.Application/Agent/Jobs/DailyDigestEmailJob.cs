using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FogBugzPd.Core.LogModule;
using log4net;

namespace FogBugzPd.Application.Agent.Jobs
{
	class DailyDigestEmailJob : JobBase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DailyDigestEmailJob));

		protected override LogEntryType LogEntryType
		{
			get { return LogEntryType.Agent_DailyDigest; }
		}

		protected override void ExecuteInternal()
		{
			_log.Info("Running task: DailyDigestJob");

			var dailyDigestService = new DailyDigestService();

			var sentCount = dailyDigestService.SendDailyMessages();

			AppendLineToLog("Sent "+ sentCount + " emails");

			_log.Info("DailyDigestJob completed");
		}
	}
}
