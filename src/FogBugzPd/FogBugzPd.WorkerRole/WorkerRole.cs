using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using FogBugzPd.Application.Agent;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using FogBugzPd.Infrastructure;
using Quartz;
using Quartz.Impl;

namespace FogBugzPd.WorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			ErrorLogUtils.Init();

			Trace.WriteLine("Start Application", "Information");

			try
			{
				ISchedulerFactory factory = new StdSchedulerFactory();
				var scheduler = factory.GetScheduler();
				scheduler.Start();
			}
			catch (SchedulerException ex)
			{
				Trace.WriteLine("SchedulerException -" + ex.Message, "Information");
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception -" + ex.Message, "Information");
			}

			while (true)
			{
				Thread.Sleep(10000);
			}
		}

		public override bool OnStart()
		{
			ServicePointManager.DefaultConnectionLimit = 12;

			return base.OnStart();
		}
	}
}
