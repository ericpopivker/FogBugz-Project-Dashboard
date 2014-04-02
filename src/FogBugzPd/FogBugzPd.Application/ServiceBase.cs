using FogBugzPd.Core;

namespace FogBugzPd.Application
{
	public class ServiceBase
	{
		protected FogBugzPdDbContext DbContext
		{
			get { return FogBugzPdDbContext.Current; }
		}
	}
}
