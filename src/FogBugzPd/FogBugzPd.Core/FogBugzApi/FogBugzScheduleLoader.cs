using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Infrastructure;
using FogLampz;
using HtmlAgilityPack;
using Group = System.Text.RegularExpressions.Group;
using FbGroup = FogBugzPd.Core.FogBugzApi.Types.Group;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace FogBugzPd.Core.FogBugzApi
{
	public class FbScheduleLoader
	{

		public FbSchedule Load(string fogBugzUrl, string token, out bool isAdmin)
		{
			if (token == String.Empty)
			{
				isAdmin = false;
				return null;
			}

			var groups = GetGroups(fogBugzUrl, token);

			/*
			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl, fogBugzUrl);

			object fbAgentCacheObject = null;

			var fbAgentCache = new FogBugzClientCache();

			if (MsCache.TryGet(cacheKey, ref fbAgentCacheObject))
			{
				fbAgentCache = (FogBugzClientCache) fbAgentCacheObject;

				isAdmin = fbAgentCache.IsLoadedByAdminUser;

				if (fbAgentCache.Schedule != null)
					return fbAgentCache.Schedule;
			}
			*/

			var siteSchedule = new Schedule();

			var holidaysHtmlNode = GetHolidaysHtmlNode(token, fogBugzUrl, out isAdmin);

			if (holidaysHtmlNode != null)
			{
				var holidaysTable = holidaysHtmlNode.SelectSingleNode("//table[@id='staticHolidayTable']");
				if (holidaysTable == null) 
					return null;

				var tableRowsOdd = holidaysTable.SelectNodes("//tr[@class='r-a ']");
				var tableRows = holidaysTable.SelectNodes("//tr[@class='row ']");

				if (tableRows != null)
				{
					foreach (var tableRowOdd in tableRowsOdd)
					{
						tableRows.Add(tableRowOdd);
					}
				}
				else
				{
					tableRows = tableRowsOdd;
				}


				if (tableRows != null)
				{
					foreach (var tableRow in tableRows)
					{
						var columns = tableRow.SelectNodes("./td");

						if (columns != null && columns.Count == 5)
						{
							var timeOffRange = GetTimeRange(columns[2].InnerText, columns[3].InnerText, columns[4].InnerText);

							siteSchedule.TimeOffRanges.Add(timeOffRange);
						}
					}
				}
			}


			
			var personalSchedules = new Dictionary<int, Schedule>();

			if (isAdmin)
			{

				foreach (var item in groups)
				{
					foreach (var person in item.Persons)
					{
						Schedule personSchedule = null;

						personalSchedules.TryGetValue(person.Index.Value, out personSchedule);

						if (personSchedule == null) personSchedule = new Schedule();

						personSchedule.TimeOffRanges.AddRange(GetGroupSchedule(siteSchedule, item));

						personalSchedules.Remove(person.Index.Value);

						personalSchedules.Add(person.Index.Value, personSchedule);
					}
				}
			}

			//fbAgentCache.Schedule = new FbSchedule() {SiteSchedule = siteSchedule};

			//MsCache.Set(cacheKey, fbAgentCache);

			return new FbSchedule() { SiteSchedule = siteSchedule, PersonSchedules = personalSchedules}; 
		}

		public IList<FbGroup> GetGroups(string fogBugzUrl, string token)
		{
			//string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl, fogBugzUrl);

			//object fbAgentCacheObject = null;

			//var fbAgentCache = new FogBugzClientCache();

			//if (MsCache.TryGet(cacheKey, ref fbAgentCacheObject))
			//{
			//	fbAgentCache = (FogBugzClientCache)fbAgentCacheObject;

			//	if (fbAgentCache.Groups != null) return fbAgentCache.Groups;
			//}
			
			if (token == String.Empty)
				return null;

			fogBugzUrl = fogBugzUrl.TrimEnd(new[] { '/' });

			var persons = FogBugzClient.GetPersons();

			var groupHtmlNode = GetGroupsHtmlNode(token, fogBugzUrl);

			var result = new List<FbGroup>();
			
			if (groupHtmlNode != null)
			{
				var groupTable = groupHtmlNode.SelectSingleNode("//table[@id='staticGroupTableFalse']");

				if (groupTable == null)
					return null;

				var tableRowsOdd = groupTable.SelectNodes("//tr[@class='r-a ']");
				var tableRows = groupTable.SelectNodes("//tr[@class='row ']");

				if (tableRows != null)
				{
					foreach (var tableRowOdd in tableRowsOdd)
					{
						tableRows.Add(tableRowOdd);
					}
				}
				else
				{
					tableRows = tableRowsOdd;
				}

				foreach (var tableRow in tableRows)
				{
					var columns = tableRow.SelectNodes("./td");

					var group = new FbGroup();

					var groupUrl = columns[2].SelectSingleNode("./a").GetAttributeValue("href", null);
					groupUrl = groupUrl.Replace("amp;", String.Empty);

					var groupName = Regex.Replace(columns[2].InnerText, "[\r\n\t]", String.Empty);
					if (groupName.ToLower().Contains("holiday"))
					{
						group.Name = groupName;
						groupUrl = fogBugzUrl + "/" + groupUrl;
						var groupHtml = GetSingleGroupHtmlNode(token, fogBugzUrl, groupUrl);
						if (groupHtml != null)
						{
							var membersTable = groupHtml.SelectSingleNode("//table[contains(@id, 'staticMembersTable')]");
							var membersTableRowsOdd = membersTable.SelectNodes("//tr[@class='r-a ']");
							var membersTableRows = membersTable.SelectNodes("//tr[@class='row ']");
							if (membersTableRows != null)
							{
								foreach (var membersTableRowOdd in membersTableRowsOdd)
								{
									membersTableRows.Add(membersTableRowOdd);
								}
							}
							else
							{
								membersTableRows = membersTableRowsOdd;
							}

							if (membersTableRows != null)
							{
								foreach (var membersTableRow in membersTableRows)
								{
									var memberColumns = membersTableRow.SelectNodes("./td");
									var userName = Regex.Replace(memberColumns[1].InnerText, "[\r\n\t ]", String.Empty);
									foreach (var person in persons)
									{
										if (person.Name == userName)
											group.Persons.Add(person);
									}
								}
							}
							result.Add(group);
						}
					}
				}
			}

			//fbAgentCache.Groups = result;
			
			//MsCache.Set(cacheKey, fbAgentCache);

			return result;
		}

		private List<TimeOffRange> GetGroupSchedule(Schedule siteSchedule, FbGroup group)
		{
			var groupName = group.Name.TrimEnd('s') + ": ";

			var timeRanges = siteSchedule.TimeOffRanges
				.Where(tr => tr.Name.IndexOf(groupName) == 0)
				.ToList();

			return timeRanges;
		}

		/*
		
		public static int GetDifference(DateTime fromDate, DateTime toDate, int? personId)
		{
			var scheduledDays = personId != null ? GetScheduledDays((int)personId, fromDate, toDate) : GetSiteScheduledDays(fromDate, toDate);
			var difference = scheduledDays.ScheduleWorkDaysCount - scheduledDays.DaysOffCount;
			return difference;
		}

		public static int GetGroupHolidaysDifference(DateTime fromDate, DateTime toDate, int personId)
		{
			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientCacheByFogBugzUrl, Url);
			object fbAgentCacheObject = null;
			var fbAgentCache = new FogBugzClientCache();
			if (MsCache.TryGet(cacheKey, ref fbAgentCacheObject))
				fbAgentCache = (FogBugzClientCache)fbAgentCacheObject;
			var fogBugzClientCache = fbAgentCache.Schedule ?? FbSchedule;

			var group = new Group();
			foreach (var groupItem in Groups)
			{
				foreach (var person in groupItem.Persons)
				{
					if (personId == person.Index)
						group = groupItem;
				}
			}

			var groupWords = group.Name.Remove(group.Name.IndexOf(':')).Split(' ');
			var groupPrefix = String.Empty;
			foreach (var groupWord in groupWords)
			{
				if (!groupWord.ToLower().Contains("holiday"))
					groupPrefix = groupWord;
			}

			int difference = 0;

			foreach (var timeOffRange in fogBugzClientCache.SiteSchedule.TimeOffRanges)
			{
				if (timeOffRange.Name.ToLower().Contains(groupPrefix))
				{
					difference = GetDifference(fromDate, toDate, null);
				}
			}
			return difference;
		}

		public static IList<TimeOffRange> GetHolidaysInRange(DateTime fromDate, DateTime toDate)
		{
			var siteScheduledDays = GetSiteScheduledDays(fromDate, toDate);
			return siteScheduledDays.TimeOffRanges;
		}
		*/
		
		
		private HtmlDocument LoadHtml(string url, CookieContainer cookieContainer)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0";
			request.CookieContainer = cookieContainer;

			string html;
			var htmlDoc = new HtmlDocument();
			var response = (HttpWebResponse)request.GetResponse();

			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				html = sr.ReadToEnd();
			}

			htmlDoc.LoadHtml(html);

			return htmlDoc;
		}

		private DateTime TryParseDateTime(string stringDateTime)
		{
			DateTime dateTime;
			if (stringDateTime.ToLower() == "today")
				return DateTime.Today;
			if (stringDateTime.ToLower() == "tomorrow")
				return DateTime.Today + new TimeSpan(1, 0, 0, 0);
			if (DateTime.TryParseExact(stringDateTime, "M/dd/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out dateTime))
			{
				return dateTime;
			}
			if (DateTime.TryParseExact(stringDateTime, "M/d/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out dateTime))
			{
				return dateTime;
			}
			if (DateTime.TryParseExact(stringDateTime, "MM/d/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out dateTime))
			{
				return dateTime;
			}
			if (DateTime.TryParseExact(stringDateTime, "MM/dd/yyyy", CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out dateTime))
			{
				return dateTime;
			}

			return dateTime;
		}
		
		private HtmlNode GetHolidaysHtmlNode(string token, string fogBugzUrl, out bool isAdmin)
		{
			var url = fogBugzUrl.Replace("https://", "");

			var cookie = new Cookie("fbToken", token, "/", url) { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0) };
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(cookie);

			var htmlDocument = LoadHtml(String.Format("https://{0}/default.asp?pg=pgWorkingSchedule", url), cookieContainer);
			var htmlNode = htmlDocument.DocumentNode;
			if (htmlNode.SelectSingleNode("//table[@id='staticHolidayTable']") != null)//if (admin==true)
			{
				isAdmin = true;
				return htmlNode;
			}

			//get ixPerson from option page
			var optionsHtml = LoadHtml(String.Format("https://{0}/default.asp?pg=pgPrefs", url), cookieContainer);
			htmlNode = optionsHtml.DocumentNode;

			var menuLink = htmlNode.SelectSingleNode("//a[@id='Menu_Options']");
			var ixPersonStr = menuLink.GetAttributeValue("href", "");
			ixPersonStr = ixPersonStr.Substring(ixPersonStr.IndexOf("&ixPerson=") + "&ixPerson=".Length);

			int ixPerson = Int32.Parse(ixPersonStr);

			htmlDocument = LoadHtml(String.Format("https://{0}/default.asp?pg=pgWorkingSchedule&ixPerson={1}", url, ixPerson), cookieContainer);
			htmlNode = htmlDocument.DocumentNode;

			isAdmin = false;
			return htmlNode;
		}

		private HtmlNode GetGroupsHtmlNode(string token, string fogBugzUrl)//selects all groups on site
		{
			var url = fogBugzUrl.Replace("https://", "").Replace("https://", "");

			var cookie = new Cookie("fbToken", token, "/", url) { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0) };
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(cookie);

			var htmlDocument = LoadHtml(String.Format("https://{0}/default.asp?pg=pgGroups", url), cookieContainer);
			var htmlNode = htmlDocument.DocumentNode;
			if (htmlNode.SelectSingleNode("//table[@id='staticGroupTableFalse']") != null)
			{
				return htmlNode;
			}

			return null;
		}

		private HtmlNode GetSingleGroupHtmlNode(string token, string fogBugzUrl, string groupUrl)//selects single group to parse
		{
			var url = fogBugzUrl.Replace("https://", "");

			var cookie = new Cookie("fbToken", token, "/", url) { Expires = DateTime.Now + new TimeSpan(365, 0, 0, 0) };
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(cookie);

			var htmlDocument = LoadHtml(groupUrl, cookieContainer);
			var htmlNode = htmlDocument.DocumentNode;
			if (htmlNode.SelectSingleNode("//table[contains(@id, 'staticMembersTable')]") != null)
			{
				return htmlNode;
			}

			return null;
		}

		private TimeOffRange GetTimeRange(string nameText, string  fromText, string toText)
		{
			var timeOffRange = new TimeOffRange();

			timeOffRange.Name = Regex.Replace(nameText, "[\r\n\t]", String.Empty);
			timeOffRange.FromDate = TryParseDateTime(fromText);
			timeOffRange.ToDate = TryParseDateTime(toText);

			return timeOffRange;
		}
	}
}