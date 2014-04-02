using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Infrastructure;
using FogLampz;
using FogLampz.Model;
using Project = FogBugzPd.Core.FogBugzApi.Types.Project;

namespace FogBugzPd.Core.FogBugzApi
{
	public static class FogBugzGateway
	{

		#region API Utils

		private static FogBugzClientCache ClientCache
		{
			get
			{
				FogBugzClientCache fbClientCache = XCallContextUtils.GetData("FogBugzClientCache") as FogBugzClientCache;

				if (fbClientCache == null)
				{
					fbClientCache=LoadFogBugzAgentCache();

					XCallContextUtils.SetData("FogBugzClientCache", fbClientCache);
				}

				return fbClientCache;
			}
		}


		private static FogBugzClientEx Client
		{
			get
			{
				FogBugzClientEx fbClient = XCallContextUtils.GetData("FogBugzClient") as FogBugzClientEx;

				if (fbClient == null)
				{
					//Try to get it from ms cache, where it is added through Login function just below
					//When first user logs in - his agent/token will be used for all other logins
					string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientByFogBugzUrl, FbAccountContext.Current.Url);

					object existingFbClient = null;
					if (MsCache.TryGet(cacheKey, ref existingFbClient))
						fbClient = (FogBugzClientEx)existingFbClient;
					else
						throw new InvalidOperationException("FogBugz Client is not available.  No user have logged in.");

					XCallContextUtils.SetData("FogBugzClient", fbClient);
				}

				return fbClient;
			}

		}

		//Load info that doesn't change very often
		private static FogBugzClientCache LoadFogBugzAgentCache()
		{
			object existingFbAgentCache=null;

			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl, FbAccountContext.Current.Url);

			if (MsCache.TryGet(cacheKey, ref existingFbAgentCache))
				return (FogBugzClientCache)existingFbAgentCache;

			var fbClient = Client;	//need to use local variable otherwise multithreading will fail since .Client is dependent on HttpContext
			var fbAgentCache = new FogBugzClientCache();

			bool allowSubProjects = FbAccountContext.Current.Settings.AllowSubProjects;
			string subProjectTag = FbAccountContext.Current.Settings.SubProjectTag;

			var url = FbAccountContext.Current.Url;
			var token = fbClient.GetToken();

			//Get data from FogBugz in parallel
			Parallel.Invoke(
				() =>
					{
						fbAgentCache.Projects = fbClient.GetProjects().Where(m => m.Name != "Inbox").ToList(); //Skip inbox for now
					},
				() =>
					{
						fbAgentCache.Milestones =
							fbClient.GetFixFors().Where(f => !f.Deleted && f.Name != "Undecided").OrderBy(m => m.DateRelease).ToList();
							//Skip undecided for now

						if (allowSubProjects)
							fbAgentCache.SubProjectParentCases = GetCasesByTag(subProjectTag, fbAgentCache.Milestones, fbClient);
					},
				() =>
					{
						fbAgentCache.Statuses = fbClient.GetStatuses().ToList();
					},

				() =>
					{
						fbAgentCache.Persons = fbClient.GetPersons().ToList();
					},
				() =>
					{
						var scheduleLoader = new FbScheduleLoader();
						bool isAdmin;

						FogBugzClient.LogOn(url+"/api.asp", token);

						fbAgentCache.Schedule = scheduleLoader.Load(url , token, out isAdmin);

						fbAgentCache.IsLoadedByAdminUser = isAdmin;
					}
				);

			//Expire in 1 day
			MsCache.Set(cacheKey, fbAgentCache, new TimeSpan(1, 0, 0, 0));
			return fbAgentCache;
		}

		#endregion

		public static FogBugzClientEx GetClientForParallel()
		{
			return Client;
		}

		public static IList<FixFor> GetMilestones()
		{
			return ClientCache.Milestones;
		}

		public static IList<Case> GetSubProjectParentCases()
		{
			return ClientCache.SubProjectParentCases;
		}

		public static IList<Status> GetStatuses()
		{
			return ClientCache.Statuses;
		}

		public static FbSchedule GetFbSchedule()
		{
			if(ClientCache != null && ClientCache.Schedule != null)
					return ClientCache.Schedule;
			return null;
		}

		public static bool GetIsScheduleLoadedByAdmin()
		{
			if (ClientCache != null)
				return ClientCache.IsLoadedByAdminUser;

			return false;
		}


		public static CaseSet GetCaseSet(int projectId, int milestoneId, int? subProjectParentCaseId = null)
		{
			object existingCaseSet = null;
			string cacheKey = MsCacheKey.GenCaseSetKey(projectId, milestoneId, subProjectParentCaseId);

			if (MsCache.TryGet(cacheKey, ref existingCaseSet))
				return (CaseSet)existingCaseSet;
			
			var caseSet = new CaseSet();

			caseSet.Project = GetProject(projectId);
			caseSet.Milestone = GetMilestone(milestoneId);

			if (subProjectParentCaseId.HasValue)
				caseSet.SubProjectParentCase = GetSubProjectParentCase(subProjectParentCaseId.Value);

			var cases = GetCases(projectId, milestoneId, subProjectParentCaseId);

			//For analytics always use cases with no children
			caseSet.Cases = cases.Where(CaseUtils.HasNoChildren).ToList();
			
			//expire in 1 hour
			MsCache.Set(cacheKey, caseSet, new TimeSpan(1, 0, 0));

			return caseSet;
		}

		public static IList<Case> GetCases(int projectId, int milestoneId, int? subProjectParentCaseId = null, int? maxCases = null, FogBugzClientEx fbClient = null)
		{
			var searchQuery = GetProjectQueryPart(projectId);
			searchQuery += " " + GetMilestoneQueryPart(milestoneId);
			searchQuery += " " + GetStatusFilter();

			if (subProjectParentCaseId.HasValue)
				searchQuery += " " + GetSubProjectQueryPart(subProjectParentCaseId.Value);

			if (fbClient == null)
				fbClient = Client;

			List<Case> cases = fbClient.GetCases(searchQuery, maxCases).ToList();
			return cases;
		}


		private static string GetStatusFilter()
		{
			var excludeStatuses = new List<string> { "Won't Implement", "Duplicate", "By Design", "Postponed" };
			return string.Join(" ", excludeStatuses.Select(part => string.Format("-status:\"Resolved ({0})\"", part)));
		}


		public static string GetSubProjectQueryPart(int subProjectParentCaseId)
		{
			string searchQuery = null;
			searchQuery = String.Format("outline:{0}", subProjectParentCaseId);
			return searchQuery;
		}


		public static IList<Case> GetCasesByTag(string tag, IList<FixFor> milestones = null, FogBugzClientEx fbClient = null)
		{
			string searchQuery = GetMilestonesQueryPart(milestones);  //limit by future milestones
			searchQuery += GetTagQueryPart(tag);

			if (fbClient == null)
				fbClient = Client;

			List<Case> cases = fbClient.GetCases(searchQuery).ToList();
			return cases;
		}


		public static Project GetProject(int projectId)
		{
			var project = GetProjects().SingleOrDefault(p => p.Index == projectId);
			if (project == null) return null;

			var newProject = new Project(project);
			return newProject;
		}

		public static Milestone GetMilestone(int milestoneId)
		{
			FixFor fixFor = GetMilestones().SingleOrDefault(m => m.Index == milestoneId);
			if (fixFor == null) return null;
			var milestone = new Milestone(fixFor);
			return milestone;
		}


		public static Case GetSubProjectParentCase(int subProjectParentCaseId)
		{
			Case subProjectParentCase = GetSubProjectParentCases().SingleOrDefault(f => f.Index == subProjectParentCaseId);
			return subProjectParentCase;
		}

		public static string GetProjectQueryPart(Project project)
		{
			return GetProjectQueryPart(project.Id);
		}

		public static string GetProjectQueryPart(int projectId)
		{
			string searchQuery = null;
			searchQuery = String.Format("project:\"={0}\"", projectId);
			return searchQuery;
		}

		public static string GetCaseQueryPart(Case caseItem)
		{
			string searchQuery = null;
			searchQuery = String.Format("case:\"={0}\"", caseItem.Index);
			return searchQuery;
		}

		public static string GetMilestonesQueryPart(IList<FixFor> milestones = null)
		{
			string searchQuery = "";
			if (milestones == null)
				milestones = GetMilestones();

			var milestoneQueryParts = new List<string>();
			foreach (var fixFor in milestones)
				milestoneQueryParts.Add(String.Format("milestone:\"={0}\"", fixFor.Index));
			
			var milestonesQuery = "(" + String.Join(" OR ", milestoneQueryParts) + ")";
			searchQuery += milestonesQuery;
			
			return searchQuery;
		}


		public static string GetMilestoneQueryPart(Milestone milestone)
		{
			return GetMilestoneQueryPart(milestone.Id);
		}

		public static string GetMilestoneQueryPart(int milestoneId)
		{
			string searchQuery = "";
			searchQuery = String.Format("milestone:\"={0}\"", milestoneId);
			return searchQuery;
		}

		private static string GetTagQueryPart(string tag)
		{
			string searchQuery = "";
			searchQuery = String.Format("tag:\"{0}\"", tag);
			return searchQuery;
		}

		public static IList<Person> GetPersons()
		{
			return ClientCache.Persons;
		}

		public static IList<FogLampz.Model.Project> GetProjects()
		{
			return ClientCache.Projects;
		}

		public static string GetToken()
		{
			return Client.GetToken();
		}



		public static string GetMilestonesQueryPart(Milestone milestone)
		{
			string searchQuery = "";
			if (milestone != null)
				searchQuery = String.Format("milestone:\"{0}\"", milestone.Name);
			else
			{
				var milestones = GetMilestones();
				var milestoneQueryParts = new List<string>();
				foreach (var fixFor in milestones)
				{
					milestoneQueryParts.Add(String.Format("milestone:\"{0}\"", fixFor.Name));
				}

				var milestonesQuery = "(" + String.Join(" OR ", milestoneQueryParts) + ")";
				searchQuery += milestonesQuery;
			}
			return searchQuery;
		}
	}
}