using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Infrastructure;
using FogLampz;
using FogLampz.Model;
using NUnit.Framework;

namespace FogBugzPd.Core.Tests.ProjectStatus
{
	public class ProjectStatusListLoaderTest
	{
		private class Credentials
		{
			public string UserName { get; set; }
			public string Password { get; set; }
			public string Url { get; set; }
		}

		private Credentials Login
		{
			get
			{

                return new Credentials
                {
                    UserName = "igor@entechsolutions.com",
                    Password = "igor12",
                    Url = "https://fogbugzpm-demo.fogbugz.com"
                };

                //return new Credentials
                //{
                //    UserName = "sasha@entechsolutions.com",
                //    Password = ",hfrfvjynt",
                //    Url = "https://entech.fogbugz.com"
                //};
			}
		}

		private FogBugzClientEx _fogBugzClient;

		private const string StatusSessionKey = "UnitTestProjectStatusLoader";

		private const string SessionKeysKey = "AllKeys";

		[SetUp]
		public void Setup()
		{
			_fogBugzClient = new FogBugzClientEx();

			FbAccountContext.GetContextFbAccountIdMethod = GetContextFbAccountId;

			DoLogin(Login);
		}


        [Test]
        public void Test_LoadList()
        {
            IList<Project> projects = FogBugzGateway.GetProjects();
            IList<FixFor> milestones = FogBugzGateway.GetMilestones();
            IList<Case> parentCases = FogBugzGateway.GetCasesByTag("ProjectParentCase");

            var projectMilestoneList = new ProjectMilestoneList(Login.Url);

            Console.WriteLine("Start get combinations");
            projectMilestoneList.GetList(projects, milestones, parentCases, 20);
            Console.WriteLine("Start get statuses");

            var projectStatusesLoader = new ProjectStatusListLoader("");
            projectStatusesLoader.SaveStatusEvent += OnSaveStatus1;
            projectStatusesLoader.BeginGettingProjectStatuses(projectMilestoneList.Items);

            Thread.Sleep(10000);

            Console.WriteLine("End");
        }

        private void OnSaveStatus1(object sender, ProjectStatusListEventArgs e)
        {
            var msg = String.Format("{0}_{1}_{2}", e.ProjectMilestone.ProjectId, e.ProjectMilestone.MileStoneId,
                                    e.ProjectMilestone.SubProjectParentCaseId.HasValue
                                        ? "_" + e.ProjectMilestone.SubProjectParentCaseId.Value
                                        : "");

            Console.WriteLine(msg);
        }

		[Test]
		public void Test_ProjectListStatuses_With_Session()
		{
			IList<Project> projects = FogBugzGateway.GetProjects();
			IList<FixFor> milestones = FogBugzGateway.GetMilestones();
			IList<Case> parentCases = FogBugzGateway.GetCasesByTag("ProjectParentCase");

			var projectMilestoneList = new ProjectMilestoneList(Login.Url);

			Console.WriteLine("Start get combinations");
			projectMilestoneList.GetList(projects, milestones, parentCases, 20);
			Console.WriteLine("Start get statuses");

			var projectStatusesLoader = new ProjectStatusListLoader("");
			projectStatusesLoader.SaveStatusEvent += OnSaveStatus;

			var keys = projectStatusesLoader.BeginGettingProjectStatuses(projectMilestoneList.Items);

			Core.MsCache.Set(SessionKeysKey, keys);
			
			Thread.Sleep(1000);

			bool isComplete = true;
			var results = GetLatestProjectStatuses(ref isComplete);
			
			Console.WriteLine("Get Latest -" + results.Values.Count() + "/" + keys.Split(',').Count());
			while (!isComplete)
			{
				Thread.Sleep(1000);
				Console.WriteLine("Get Latest -" + results.Values.Count() + "/" + keys.Split(',').Count());
				results = GetLatestProjectStatuses(ref isComplete);
			}

			ClearResults();

			Console.WriteLine("End");
		}

		private void DoLogin(Credentials credentials)
		{
			_fogBugzClient.LogOn(credentials.Url + "/api.asp", credentials.UserName, credentials.Password);

			XCallContextUtils.SetData("FogBugzClient", _fogBugzClient);

			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientByFogBugzUrl, Login.Url);

			MsCache.Set(cacheKey, _fogBugzClient);

			MsCache.Set("FogBugzClient", _fogBugzClient);
		}

		private int GetContextFbAccountId()
		{
			return 2; //change it for local testing
		}

		private void OnSaveStatus(object sender, ProjectStatusListEventArgs e)
		{
			var key = String.Format("{0}_{1}_{2}{3}", StatusSessionKey, e.ProjectMilestone.ProjectId, e.ProjectMilestone.MileStoneId,
			                        e.ProjectMilestone.SubProjectParentCaseId.HasValue
				                        ? "_" + e.ProjectMilestone.SubProjectParentCaseId.Value
				                        : "");

			Core.MsCache.Set(key, e.Status);
		}

		private Dictionary<ProjectStatusListItem, FogbugzProjectStatus> GetLatestProjectStatuses(ref bool isComplete)
		{
			object storedKeysObject = null;
			string keys = string.Empty;
			
			if (Core.MsCache.TryGet(SessionKeysKey, ref storedKeysObject))
				keys = (string)storedKeysObject;
			

			var result = new Dictionary<ProjectStatusListItem, FogbugzProjectStatus>();

			if (!String.IsNullOrEmpty(keys))
			{
				var splitKeys = keys.Split(',');

				foreach (var key in splitKeys)
				{
					var sessionKey = StatusSessionKey + "_" + key;
					object storedStatusObject = null;

					if (Core.MsCache.TryGet(sessionKey, ref storedStatusObject))
					{

						var status = (Core.ProjectStatus.ProjectStatus)storedStatusObject;

						var ids = key.Split('_');
						var projectMilesToneItem = new ProjectStatusListItem();
						projectMilesToneItem.ProjectId = Convert.ToInt32(ids[0]);
						projectMilesToneItem.MileStoneId = Convert.ToInt32(ids[1]);
						if (ids.Count() > 2)
							projectMilesToneItem.SubProjectParentCaseId = Convert.ToInt32(ids[2]);
						result.Add(projectMilesToneItem, status.Status);
					}
				}

				isComplete = splitKeys.Count() == result.Count;
			}

			

			return result;
		}

		private void ClearResults()
		{
			object storedKeysObject = null;
			string keys = string.Empty;

			if (Core.MsCache.TryGet(SessionKeysKey, ref storedKeysObject))
				keys = (string)storedKeysObject;

			foreach (var key in keys.Split(','))
			{
				Core.MsCache.TryRemove(key);
			}

			Core.MsCache.TryRemove(SessionKeysKey);
		}
	}
}
