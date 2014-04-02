using System.Linq;
using System.Text.RegularExpressions;
using FogBugzPd.Core;
using FogLampz.Model;

namespace FogBugzPd.Core.FogBugzApi
{
	public static class CaseUtils
	{
		#region Case Status predicates
		
		/*private static readonly Regex ResolvedNotVerifiedRegex = new Regex(
			@"^Resolved(?!\s*\(Verified)",
			RegexOptions.Singleline | RegexOptions.Compiled);*/

		public static bool IsActive(Case arg)
		{
			Status status = FogBugzGateway.GetStatuses().FirstOrDefault(s => s.Index == arg.IndexStatus);

			if (status == null) return false;

			return !status.WorkDone && !status.Resolved && !status.Deleted && !status.Duplicate;
		}

		public static bool IsResolved(Case arg)
		{
			Status status = FogBugzGateway.GetStatuses().FirstOrDefault(s => s.Index == arg.IndexStatus);

			if (status == null) return false;

			return status.Resolved && arg.IsOpen && !IsResolvedVerified(arg);
		}

		/*public static bool IsResolvedNotVerified(Case arg)
		{
			Status status = FogBugzUtils.GetStatuses().First(s => s.Index == arg.IndexStatus);
			return ResolvedNotVerifiedRegex.IsMatch(status.Name) && arg.IsOpen;
		}*/

		public static bool IsResolvedVerified(Case arg)
		{
			//Status status = FogBugzGateway.GetStatuses().First(s => s.Index == arg.IndexStatus);

			bool isResolvedVerified = FbAccountContext.Current.Settings.ResolvedVerifiedStatusId == arg.IndexStatus;

			return isResolvedVerified && arg.IsOpen;
		}
		
		public static bool IsClosed(Case arg)
		{
			return !arg.IsOpen;
		}

		public static bool HasNoChildren(Case arg)
		{
			return (arg.IndexBugChildren == null || arg.IndexBugChildren.Count() == 0);
		}

		#endregion
	}
}
