namespace FogBugzPd.Core
{
	public enum MsCacheDataType
	{
		FogBugz_ClientByFogBugzUrl = 1,
		FogBugz_ClientCacheByFogBugzUrl = 2,
		FogBugz_CaseSet,
		FbAccount,

		FB_CasesByQuery,
		FB_FixFors,
		FB_Statuses,
		FB_CaseSet,
		FB_Persons,
		FB_TagsByMilestone,
		TR_Milestones,
		TR_Milestone,
		TR_Projects,
		TR_Plans,
		TR_Runs,
		TR_Tests,
		TR_Case,
		TR_Cases,
		TR_Suites,

		ProjectLists,
		ProjectsStatuses
	}

	public static class MsCacheKey
	{
		public static string Gen(MsCacheDataType msCacheDataType, string key)
		{
			return msCacheDataType.ToString() + "_" + key;
		}


		public static string GenCaseSetKey(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			string key = FbAccountContext.Current.Url + "__" + projectId + "__" + milestoneId;
			if (subProjectParentCaseId.HasValue)
				key += "__" + subProjectParentCaseId;

			return MsCacheKey.Gen(MsCacheDataType.FogBugz_CaseSet, key);
		}
		 
	}
}