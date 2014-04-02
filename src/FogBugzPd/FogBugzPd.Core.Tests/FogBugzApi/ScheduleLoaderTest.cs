using System;
using System.Collections.Generic;
using System.Diagnostics;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogLampz;
using FogLampz.Model;
using NUnit.Framework;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Core.Tests.FogBugzApi
{

	[TestFixture]
	public class ScheduleLoaderTest
	{
		class Credentials
		{
			public string UserName { get; set; }
			public string Password { get; set; }
			public string Url { get; set; }
		}

		private Credentials UserCredentials
		{
			get
			{

				return new Credentials
				{
					UserName = "igor@entechsolutions.com",
					Password = "igor12",
					Url = "https://fogbugzpm-demo.fogbugz.com/api.asp"
				};
			}
		}

		private Credentials AdminCredentials
		{
			get
			{

				return new Credentials
				{
					UserName = "fogbugzpm-demo@entechsolutions.com",
					Password = "fogbugzpm-demo",
					Url = "https://fogbugzpm-demo.fogbugz.com/api.asp"
				};
			}
		}

		[Test]
		public void TryLoadSiteSchedule_AsAdmin_ReturnSchedule()
		{
			FogBugzClient.LogOn(AdminCredentials.Url, AdminCredentials.UserName, AdminCredentials.Password);

			var token = FogBugzClient.GetToken();

			var scheduleLoader = new FbScheduleLoader();

			bool isAdmin;

			var fbSchedule = scheduleLoader.Load("https://fogbugzpm-demo.fogbugz.com", token, out isAdmin);

			Assert.IsTrue(isAdmin, "isAdmin");
			Assert.IsNotNull(fbSchedule.SiteSchedule);
			Assert.IsTrue(fbSchedule.SiteSchedule.TimeOffRanges.Count >0 );
		}

		[Test]
		public void TryLoadSiteSchedule_AsUser_ReturnSchedule()
		{
			FogBugzClient.LogOn(UserCredentials.Url, UserCredentials.UserName, UserCredentials.Password);

			var token = FogBugzClient.GetToken();

			var scheduleLoader = new FbScheduleLoader();

			bool isAdmin;

			var fbSchedule = scheduleLoader.Load("https://fogbugzpm-demo.fogbugz.com", token, out isAdmin);

			Assert.IsTrue(!isAdmin, "isAdmin");
			Assert.IsNotNull(fbSchedule.SiteSchedule);
			Assert.IsTrue(fbSchedule.SiteSchedule.TimeOffRanges.Count > 0);
		}

		[Test]
		public void TryGetGroups_AsAdmin_ReturnGroups()
		{
			FogBugzClient.LogOn(AdminCredentials.Url, AdminCredentials.UserName, AdminCredentials.Password);

			var token = FogBugzClient.GetToken();

			var scheduleLoader = new FbScheduleLoader();

			var groups = scheduleLoader.GetGroups("https://fogbugzpm-demo.fogbugz.com", token);

			Assert.IsNotNull(groups);
			Assert.IsTrue(groups.Count > 0);
		}

		[Test]
		public void TryGetGroups_AsUser_ReturnNull()
		{
			FogBugzClient.LogOn(UserCredentials.Url, UserCredentials.UserName, UserCredentials.Password);

			var token = FogBugzClient.GetToken();

			var scheduleLoader = new FbScheduleLoader();

			var groups = scheduleLoader.GetGroups("https://fogbugzpm-demo.fogbugz.com", token);

			Assert.IsTrue(groups.Count == 0);
		}

		[Test]
		public void TryGetSiteScheduledDays_ReturnScheduledDaysResult()
		{
			var fbSchedule = new FbSchedule();

			List<TimeOffRange> holidays = new List<TimeOffRange>();
			
			holidays.Add(new TimeOffRange() {FromDate = new DateTime(2013, 12, 24), ToDate = new DateTime(2013, 12, 31), Name = "Christmas" });

			fbSchedule.SiteSchedule.TimeOffRanges = holidays;

			var result = fbSchedule.GetSiteScheduledDays(new DateTime(2013, 12, 1), new DateTime(2013, 12, 31));

			Assert.IsNotNull(result);
			Assert.IsTrue(result.DaysOffCount == 5);
		}
	}
}
