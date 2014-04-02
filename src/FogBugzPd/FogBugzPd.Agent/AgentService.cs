using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Utils;
using Quartz;
using Quartz.Impl;
using log4net;

namespace FogBugzPd.Agent
{
	public partial class AgentService : ServiceBase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(AgentService));
		
		public AgentService()
		{
			InitializeComponent();

			this.ServiceName = "FogBugzPd Agent";
		}

		protected override void OnStart(string[] args)
		{
			_log.Debug("OnStart");

			ISchedulerFactory factory = new StdSchedulerFactory();

			IScheduler scheduler = factory.GetScheduler();
			scheduler.Start();
		}

		protected override void OnStop()
		{
			_log.Debug("OnStop");
		}
	}
}
