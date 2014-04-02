using System;
using System.Collections.Generic;
using System.Diagnostics;
using FogBugzPd.Core.FogBugzApi;
using FogLampz;
using FogLampz.Model;
using NUnit.Framework;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Core.Tests.FogBugzApi
{

	[TestFixture]
	public class ProjectMilestoneListTest
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
						UserName = "sasha@entechsolutions.com",
						Password = "lpfujtd1",
						Url = "https://gramercyone.fogbugz.com"
					};
			}
		}

		private FogBugzClientEx _fogBugzClient;

		[SetUp]
		public void Setup()
		{

			_fogBugzClient = new FogBugzClientEx();

			FbAccountContext.GetContextFbAccountIdMethod = GetContextFbAccountId;
			
			DoLogin(Login);

		}

		private void DoLogin(Credentials credentials)
		{
			_fogBugzClient.LogOn(credentials.Url + "/api.asp", credentials.UserName, credentials.Password);

			XCallContextUtils.SetData("FogBugzClient", _fogBugzClient);

			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientByFogBugzUrl, Login.Url);

			MsCache.Set(cacheKey, _fogBugzClient);
		}

		[Test]
		public void Test_Retrieving_Combinations()
		{
			IList<Project> projects = FogBugzGateway.GetProjects();
			IList<FixFor> milestones = FogBugzGateway.GetMilestones();
			IList<Case> parentCases = FogBugzGateway.GetCasesByTag("ProjectParentCase");

		
			//for (int i = 1; i < 2; i++)
			//{
				RunTest(projects, milestones, parentCases, 1);
				RunTest(projects, milestones, parentCases, 3);
				RunTest(projects, milestones, parentCases, 10);
				RunTest(projects, milestones, parentCases, 20);
				RunTest(projects, milestones, parentCases, 30);
			//}

			
		}

		private void RunTest(IList<Project> projects, IList<FixFor> milestones,
		                    IList<Case> parentCases, int threadsCount)
		{
			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();
			var projectMilestoneList = new ProjectMilestoneList(Login.Url);
			projectMilestoneList.GetList(projects, milestones, parentCases, threadsCount);
			stopwatch.Stop();

			Console.WriteLine("Thread: " + threadsCount + ", Duration: " + stopwatch.ElapsedMilliseconds);
			Console.WriteLine("=========================================================================");
			//Assert.IsNotNull(projectMilestoneList.Items);
		}


		private int GetContextFbAccountId()
		{
			return 2; //change it for local testing
		}
	}

}
