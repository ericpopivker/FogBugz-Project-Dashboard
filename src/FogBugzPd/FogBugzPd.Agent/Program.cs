using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Quartz;
using Quartz.Impl;
using log4net;

namespace FogBugzPd.Agent
{
	internal static class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(Program));
		
		private static void Main()
		{
			log4net.Config.XmlConfigurator.Configure();

			var args = new List<string>(Environment.GetCommandLineArgs());

			if (args.Count > 1)
			{
				if (args.Count == 2 && args[1] == "-CommandLine")
				{
					RunCommandLine();
				}
				if (args.Count == 2 && args[1].StartsWith("-Job:"))
				{
					RunJob(args[1]);
				}

			}
			else
			{
				RunService();
			}
		}


		private static void RunCommandLine()
		{
			ISchedulerFactory factory = new StdSchedulerFactory();

			var scheduler = factory.GetScheduler();
			scheduler.Start();

			while (true)
			{
				System.Threading.Thread.Sleep(3600000);
			}
		}

		private static void RunService()
		{
			var servicesToRun = new ServiceBase[]
				{
					new AgentService() 
				};

			ServiceBase.Run(servicesToRun);
		}

		private static void RunJob(string s)
		{
			Application.Agent.AgentUtils.RunJobByName(s.ParseJobName());
		}

		static string ParseJobName(this string arg)
		{
			return arg.Split(':')[1];
		}
	}
}
