using System;
using log4net;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Application.Agent
{
	public class AgentUtils
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(AgentUtils));

		public static void HandleException(Exception e, string jobName = null)
		{
			_log.Error("Agent Exception", e);
		}

		public static void RunJobByName(string name)
		{
			var jobType = Type.GetType(typeof(AgentUtils).Namespace + ".Jobs." + name);

			if (jobType == null)
				throw new InvalidOperationException("Invalid job name: " + name);

			var job = (JobBase)Activator.CreateInstance(jobType);
			job.Execute();
		}
	}
}
