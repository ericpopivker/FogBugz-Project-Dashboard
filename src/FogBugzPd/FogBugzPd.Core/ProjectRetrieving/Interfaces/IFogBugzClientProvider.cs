using FogLampz;

namespace FogBugzPd.Core.ProjectRetrieving
{
	public interface IFogBugzClientProvider
	{
		FogBugzClientEx GetClient(string key);
	}
}