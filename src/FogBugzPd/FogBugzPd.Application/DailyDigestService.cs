using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FogBugzPd.Application.Agent;
using FogBugzPd.Core.FbAccountModule;
using FogBugzPd.Core.LogModule;
using FogBugzPd.Web.Utils;
using log4net;

namespace FogBugzPd.Application
{
	public class DailyDigestService : ServiceBase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DailyDigestService));

		public int SendDailyMessages()
		{
			var fbAccounts = DbContext.FbAccounts
				.Include("Settings")
				.Where(f=>f.Settings.SendDailyDigestEmails)
				.ToList();

			int count = 0;

			foreach (var fbAccount in fbAccounts)
			{
				var request = (HttpWebRequest)WebRequest.Create(String.Format("{0}/Agent/SendDailyDigestEmails/{1}", EnvironmentConfig.GetFrontEndWebRootUrl(), fbAccount.Settings.Guid.Value));

				var response = (HttpWebResponse)request.GetResponse();

				count++;
			}

			_log.Info("DailyDigestService completed");

			return count;
		}

	}
}
