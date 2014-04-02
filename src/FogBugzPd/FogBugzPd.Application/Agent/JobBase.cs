using System;
using System.Text;
using FogBugzPd.Core;
using FogBugzPd.Core.LogModule;
using Quartz;

namespace FogBugzPd.Application.Agent
{
	public abstract class JobBase : IJob
	{
		protected LogService _logService;
		private StringBuilder _logEntryDescription;

		protected FogBugzPdDbContext DbContext
		{
			get { return FogBugzPdDbContext.Current; }
		}

		protected abstract LogEntryType LogEntryType { get;}

		protected JobBase()
		{
			_logService=new LogService();
			 _logEntryDescription= new StringBuilder();
		}

		public void Execute()
		{
			using (new FogBugzPdDbContextScope())
			{
				DateTime startTime = DateTime.Now;

				try
				{
					ExecuteInternal();
				}
				catch (Exception e)
				{
					AgentUtils.HandleException(e);
				}

				string description = "Started: " + startTime + Environment.NewLine;

				TimeSpan timeSpan = DateTime.Now - startTime;
				description += "Duration: " + timeSpan.TotalSeconds + " secs";

				if (_logEntryDescription.Length > 0)
				{
					description += Environment.NewLine + Environment.NewLine;
					description += _logEntryDescription.ToString();
				}

				_logService.AddEntry(this.LogEntryType, description);
			}
		}

		protected abstract void ExecuteInternal();


		public void Execute(JobExecutionContext context)
		{
			Execute();
		}

		protected void AppendLineToLog(string line)
		{
			_logEntryDescription.AppendLine(line);
		}
	}
}
