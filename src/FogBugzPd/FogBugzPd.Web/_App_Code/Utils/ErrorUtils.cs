using System;
using FogBugzPd.Core;
using FogBugzPd.Infrastructure;
using log4net;

namespace FogBugzPd.Web.Utils
{
	//Common web utils used on all web apps
	public class ErrorUtils
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(ErrorUtils));

		public static void HandleException(Exception exception)
		{
		
			_log.Error("Agent Exception", exception);
		}
	}
}