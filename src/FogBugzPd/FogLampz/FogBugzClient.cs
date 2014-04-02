// -----------------------------------------------------------------------
//  <copyright file="FogBugzClient.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using FogLampz.Exceptions;
using FogLampz.Model;
using FogLampz.Util;

#endregion

namespace FogLampz
{
	//Obsolete.  Use FogBugzClientEx instead in FogBugzPm
	public static class FogBugzClient
	{
		private static readonly IDictionary<int, Status> StatusCache = new Dictionary<int, Status>();
		private static readonly IDictionary<int, Category> CategoryCache = new Dictionary<int, Category>();
		private static readonly IDictionary<int, Project> ProjectCache = new Dictionary<int, Project>();
		private static readonly IDictionary<int, Priority> PriorityCache = new Dictionary<int, Priority>();
		private static readonly IDictionary<int, Person> PersonCache = new Dictionary<int, Person>();
		private static readonly IDictionary<int, Area> AreaCache = new Dictionary<int, Area>();

		/// <summary>
		///   Gets the client.
		/// </summary>
		private static RestClient Client { get; set; }

		/// <summary>
		///   Initializes the FogBugz rest client with the specified credentials.
		/// </summary>
		/// <param name="url"> The URL. </param>
		/// <param name="email"> The email. </param>
		/// <param name="password"> The password. </param>
		public static void LogOn(string url, string email, string password)
		{
			LogOn(new Uri(url), email, password);
		}

		/// <summary>
		///   Initializes the FogBugz rest client with the specified credentials.
		/// </summary>
		/// <param name="uri"> The URI. </param>
		/// <param name="email"> The email. </param>
		/// <param name="password"> The password. </param>
		public static void LogOn(Uri uri, string email, string password)
		{
			if ((uri != null &&
			     !string.IsNullOrEmpty(uri.AbsoluteUri)
			     && !string.IsNullOrEmpty(email)
			     && !string.IsNullOrEmpty(password)
			     && email.Contains('@')))
			{
				Client = new RestClient(uri.AbsoluteUri, email, password);
			}
			else
			{
				throw new NotLoggedInException("Unable to log on to the FogBougz API. please verify the supplied credentials.");
			}
		}

		public static void LogOn(string url, string token)
		{
			Client = new RestClient(url, token);
		}

		public static string GetToken()
		{
			if (Client == null) return null;
			return Client.Token;
		}

		/// <summary>
		///   Initializes the caches.
		/// </summary>
		public static void InitializeCaches()
		{
			GetStatuses(true);
			GetCategories(true);
			GetProjects(true);
			GetPriorities(true);
			GetPersons(true);
			GetAreas(true);
		}

		/// <summary>
		///   Gets the filters.
		/// </summary>
		/// <returns> </returns>
		public static IEnumerable<Filter> GetFilters()
		{
			return List<Filter>();
		}

		public static void SetFilter(int filter)
		{
		}

		/// <summary>
		///   Gets the case.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> </returns>
		public static Case GetCase(int index)
		{
			return GetCases(new[] {index}).FirstOrDefault();
		}

		/// <summary>
		///   Gets the cases.
		/// </summary>
		/// <param name="caseIndexes"> The case indices. </param>
		/// <returns> </returns>
		public static IEnumerable<Case> GetCases(IEnumerable<int> caseIndexes)
		{
			return Search<Case>(string.Join(",", caseIndexes));
		}

		public static IEnumerable<Case> GetCases(string q)
		{
			return Search<Case>(q);
		}

		public static IEnumerable<Case> GetCases(string q, string cols)
		{
			return Search<Case>(q, cols);
		}

		/// <summary>
		///   Creates the case.
		/// </summary>
		/// <param name="case"> The @case. </param>
		/// <returns> </returns>
		public static Case CreateCase(Case @case)
		{
			return Create(@case);
		}

		/// <summary>
		///   Gets the category.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> the category or <c>null</c> if the category was not found. </returns>
		public static Category GetCategory(int index)
		{
			if (!CategoryCache.ContainsKey(index))
			{
				GetCategories(true);
			}

			return CategoryCache.ContainsKey(index) ? CategoryCache[index] : null;
		}

		/// <summary>
		///   Gets the categories.
		/// </summary>
		/// <returns> a list of categories </returns>
		public static IEnumerable<Category> GetCategories()
		{
			return GetCategories(false);
		}

		/// <summary>
		///   Gets the categories.
		/// </summary>
		/// <param name="refreshCache"> if set to <c>true</c> the cache will be refreshed. </param>
		/// <returns> a list of categories </returns>
		public static IEnumerable<Category> GetCategories(bool refreshCache)
		{
			if (CategoryCache.Count > 0 && !refreshCache)
			{
				return CategoryCache.Select(kvp => kvp.Value);
			}

			var list = List<Category>();

			CategoryCache.Clear();
			foreach (var category in list)
			{
				if (category.Index.HasValue)
					CategoryCache.Add(category.Index.Value, category);
			}

			return list;
		}

		/// <summary>
		///   Gets the person.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> the specified person or <c>null</c> if the person does not exist. </returns>
		public static Person GetPerson(int index)
		{
			if (!PersonCache.ContainsKey(index))
			{
				GetPersons(true);
			}
			return PersonCache.ContainsKey(index) ? PersonCache[index] : null;
		}

		/// <summary>
		///   Gets the persons.
		/// </summary>
		/// <returns> a list of persons </returns>
		public static IEnumerable<Person> GetPersons()
		{
			return GetPersons(false);
		}

		/// <summary>
		///   Gets the persons.
		/// </summary>
		/// <param name="refreshCache"> if set to <c>true</c> the cache will be refreshed. </param>
		/// <returns> a list of persons </returns>
		public static IEnumerable<Person> GetPersons(bool refreshCache)
		{
			if (PersonCache.Count > 0 && !refreshCache)
			{
				return PersonCache.Select(kvp => kvp.Value);
			}

			var args = new Dictionary<string, string>();
			args.Add("fIncludeActive", "1");
			args.Add("fIncludeDeleted","1");

			var list = List<Person>(args);

			PersonCache.Clear();
			PersonCache.Add(1, Person.ClosedUser);
			foreach (var person in list)
			{
				if (person.Index.HasValue)
					PersonCache.Add(person.Index.Value, person);
			}
			return PersonCache.Select(kvp => kvp.Value);
		}

		/// <summary>
		///   Creates the fix for.
		/// </summary>
		/// <param name="fixFor"> The fix for. </param>
		/// <returns> </returns>
		public static FixFor CreateFixFor(FixFor fixFor)
		{
			return Create(fixFor);
		}

		/// <summary>
		///   Gets the fix fors.
		/// </summary>
		/// <param name="projectId"> </param>
		/// <returns> </returns>
		public static IEnumerable<FixFor> GetFixFors(int projectId=0)
		{
			var args = new Dictionary<string, string>();
			args.Add("fIncludeDeleted", "0");
			args.Add("fIncludeReallyDeleted", "0");
			//args.Add("ixProject", "0");//, "0");
			return List<FixFor>(args);
		}

		/// <summary>
		///   Gets the area.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> the area or <c>null</c> if the area was not found. </returns>
		public static Area GetArea(int index)
		{
			if (!AreaCache.ContainsKey(index))
			{
				GetAreas(true);
			}
			return AreaCache.ContainsKey(index) ? AreaCache[index] : null;
		}

		/// <summary>
		///   Creates the area.
		/// </summary>
		/// <param name="area"> The area. </param>
		/// <returns> </returns>
		public static Area CreateArea(Area area)
		{
			return Create(area);
		}

		/// <summary>
		///   Gets the areas.
		/// </summary>
		/// <returns> the list of areas </returns>
		public static IEnumerable<Area> GetAreas()
		{
			return GetAreas(false);
		}

		/// <summary>
		///   Gets the areas.
		/// </summary>
		/// <param name="refreshCache"> if set to <c>true</c> the cache will be refreshed. </param>
		/// <returns> the list of areas </returns>
		public static IEnumerable<Area> GetAreas(bool refreshCache)
		{
			if (AreaCache.Count > 0 && !refreshCache)
			{
				return AreaCache.Select(kvp => kvp.Value);
			}

			var list = List<Area>();

			AreaCache.Clear();
			foreach (var item in list)
			{
				if (item.Index.HasValue)
					AreaCache.Add(item.Index.Value, item);
			}

			return list;
		}

		/// <summary>
		///   Gets the priority.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> the specified priority or <c>null</c> if the priority does not exist </returns>
		public static Priority GetPriority(int index)
		{
			if (!PriorityCache.ContainsKey(index))
			{
				GetPriorities(true);
			}

			return PriorityCache.ContainsKey(index) ? PriorityCache[index] : null;
		}

		/// <summary>
		///   Gets the priorities.
		/// </summary>
		/// <returns> the list of priorities </returns>
		public static IEnumerable<Priority> GetPriorities()
		{
			return GetPriorities(false);
		}

		/// <summary>
		///   Gets the priorities.
		/// </summary>
		/// <param name="refreshCache"> if set to <c>true</c> the cache will be refreshed. </param>
		/// <returns> the list of priorities </returns>
		public static IEnumerable<Priority> GetPriorities(bool refreshCache)
		{
			if (PriorityCache.Count > 0 && !refreshCache)
			{
				return PriorityCache.Select(kvp => kvp.Value);
			}

			var list = List<Priority>();

			PriorityCache.Clear();
			foreach (var item in list)
			{
				if (item.Index.HasValue)
					PriorityCache.Add(item.Index.Value, item);
			}

			return list;
		}

		/// <summary>
		///   Creates the project.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <returns> </returns>
		public static Project CreateProject(Project project)
		{
			return Create(project);
		}

		/// <summary>
		///   Gets the project.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> the specified project, or null of the project does not exist </returns>
		public static Project GetProject(int index)
		{
			if (!ProjectCache.ContainsKey(index))
			{
				GetProjects(true);
			}

			return ProjectCache.ContainsKey(index) ? ProjectCache[index] : null;
		}

		/// <summary>
		///   Gets the projects.
		/// </summary>
		/// <returns> the list of projects </returns>
		public static IEnumerable<Project> GetProjects()
		{
			return GetProjects(false);
		}

		/// <summary>
		///   Gets the projects.
		/// </summary>
		/// <param name="refreshCache"> if set to <c>true</c> the cache will be refreshed. </param>
		/// <returns> the list of projects </returns>
		public static IEnumerable<Project> GetProjects(bool refreshCache)
		{
			if (ProjectCache.Count > 0 && !refreshCache)
			{
				return ProjectCache.Select(kvp => kvp.Value);
			}

			var list = List<Project>();

			ProjectCache.Clear();
			foreach (var project in list)
			{
				if (project.Index.HasValue)
					ProjectCache.Add(project.Index.Value, project);
			}

			return list;
		}

		/// <summary>
		///   Gets the status.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns> the specified status or <c>NULL</c> if the status does not exist </returns>
		public static Status GetStatus(int index)
		{
			if (!StatusCache.ContainsKey(index))
			{
				GetStatuses(true);
			}

			return StatusCache.ContainsKey(index) ? StatusCache[index] : null;
		}

		/// <summary>
		///   Gets the statuses.
		/// </summary>
		/// <returns> a list of all status objects </returns>
		public static IEnumerable<Status> GetStatuses()
		{
			return GetStatuses(false);
		}

		/// <summary>
		///   Gets the statuses.
		/// </summary>
		/// <param name="refreshCache"> if set to <c>true</c> the cache will be refreshed. </param>
		/// <returns> a list of all status objects </returns>
		public static IEnumerable<Status> GetStatuses(bool refreshCache)
		{
			if (StatusCache.Count > 0 && !refreshCache)
			{
				return StatusCache.Select(kvp => kvp.Value);
			}

			var list = List<Status>();

			StatusCache.Clear();
			foreach (var status in list)
			{
				if (status.Index.HasValue)
					StatusCache.Add(status.Index.Value, status);
			}

			return list;
		}

		/// <summary>
		///   Gets the mailboxes.
		/// </summary>
		/// <returns> </returns>
		public static IEnumerable<Mailbox> GetMailboxes()
		{
			return List<Mailbox>();
		}

		/// <summary>
		///   Creates the specified entity.
		/// </summary>
		/// <typeparam name="TEntity"> The type of the entity. </typeparam>
		/// <param name="entity"> The entity. </param>
		/// <returns> the created entity </returns>
		/// <exception cref="FogBugzException">An error was encountered creating the entity.</exception>
		private static TEntity Create<TEntity>(TEntity entity)
			where TEntity : IFogBugzEntity, new()
		{
			if (string.IsNullOrEmpty(entity.ApiInfo.CreateCommand))
				throw new InvalidOperationException("Entity does not support the create operation");

			var res = Client.Command(entity.ApiInfo.CreateCommand, entity.GetPropertyValues());
			var reader = res.GetXmlReader();
			return ParseEntityXml<TEntity>(reader, entity.ApiInfo.Root, entity.ApiInfo.Element).FirstOrDefault();
		}

		/// <summary>
		///   Invokes the list operation for the specified entity
		/// </summary>
		/// <typeparam name="TEntity"> The type of the entity. </typeparam>
		/// <returns> the result list </returns>
		private static IList<TEntity> List<TEntity>(IDictionary<string,string> arguments = null)
			where TEntity : IFogBugzEntity, new()
		{
			var apiInfo = new TEntity().ApiInfo;
			var response = Client.Command(apiInfo.ListCommand,arguments);
			using (var stringReader = new StringReader(response.Trim()))
			{
				using (var xmlReader = XmlReader.Create(stringReader))
				{
					xmlReader.MoveToContent();
					return ParseEntityXml<TEntity>(xmlReader, apiInfo.Root, apiInfo.Element).ToList();
				}
			}
		}

		//private static IList<TEntity> List<TEntity>()
		//    where TEntity : IFogBugzEntity, new()
		//{
		//    var apiInfo = new TEntity().ApiInfo;
		//    var response = Client.Command(apiInfo.ListCommand);
		//    var fileText = File.ReadAllText("Z:\\test.xml");
		//    using (var stringReader = new StringReader(fileText.Trim()))
		//    {
		//        using (var xmlReader = XmlReader.Create(stringReader))
		//        {
		//            xmlReader.MoveToContent();
		//            return ParseEntityXml<TEntity>(xmlReader, apiInfo.Root, apiInfo.Element).ToList();
		//        }
		//    }
		//}

		/// <summary>
		///   Invokes a search for the specified entity, using the specified query.
		/// </summary>
		/// <typeparam name="TEntity"> The type of the entity. </typeparam>
		/// <param name="s"> </param>
		/// <param name="query"> The query. </param>
		/// <returns> the result list. </returns>
		private static IList<TEntity> Search<TEntity>(string query, string cols = null)
			where TEntity : IFogBugzEntity, new()
		{
			var apiInfo = new TEntity().ApiInfo;
			var response = Client.Search(query, cols);
			using (var stringReader = new StringReader(response.Trim()))
			{
				using (var xmlReader = XmlReader.Create(stringReader))
				{
					xmlReader.MoveToContent();
					return ParseEntityXml<TEntity>(xmlReader, apiInfo.Root, apiInfo.Element).ToList();
				}
			}
		}

		/// <summary>
		///   Parses the entity XML.
		/// </summary>
		/// <typeparam name="TEntity"> The type of the entity. </typeparam>
		/// <param name="reader"> The reader. </param>
		/// <param name="root"> The root. </param>
		/// <param name="element"> The element. </param>
		/// <returns> the parsed entity </returns>
		private static IEnumerable<TEntity> ParseEntityXml<TEntity>(XmlReader reader, string root, string element)
			where TEntity : IFogBugzEntity, new()
		{
			if (string.IsNullOrEmpty(root) || string.IsNullOrEmpty(element))
				throw new InvalidOperationException("Invalid operation. entity contains no root or element header");

			if (reader == null)
				yield break;

			reader.Read();
			while (!reader.EOF)
			{
				if (reader.NodeType != XmlNodeType.Element)
				{
					if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "response")
						yield break;
					reader.Read();
					continue;
				}
				var name = reader.Name;
				if (name == null)
				{
					reader.Read();
					continue;
				}

				if (name == "error")
				{
					throw reader.GetErrorExceptionInfo();
				}

				if (name == element)
				{
					var fields = reader.GetSubElementValues();

					var entity = new TEntity();
					entity.Initialize(fields);

					yield return entity;

					continue;
				}
				if (name == root)
				{
					reader.Read();
				}
			}
		}

		#region Nested type: RestClient

		private class RestClient
		{
			private static readonly List<string> DefaultColumns = new List<string>
				{
					"ixBug",
					"ixBugParent",
					"ixBugChildren",
					"sTitle",
					"ixProject",
					"sProject",
					"ixArea",
					"sArea",
					"tags",
					"ixPersonAssignedTo",
					"sPersonAssignedTo",
					"ixPersonOpenedBy",
					"ixPersonResolvedBy",
					"ixPersonClosedBy",
					"ixPersonLastEditedBy",
					"ixStatus",
					"ixPriority",
					"sPriority",
					"ixFixFor",
					"sFixFor",
					"hrsOrigEst",
					"hrsCurrEst",
					"hrsElapsed",
					"ixCategory",
					"dtOpened",
					"dtResolved",
					"dtClosed",
					"dtLastUpdated",
					"dtDue",
					"ixBugEventLatest",
					"ixBugEventLastView",
					"plugin",
					"plugin_customfields",
					"fOpen",
					"fAssignable",
					"fDeleted"
				};

			private readonly string _url;
			private bool _attemptingLogin;
			private string _token;

			/// <summary>
			///   Initializes a new instance of the <see cref="RestClient" /> class.
			/// </summary>
			/// <param name="url"> The URL. </param>
			/// <param name="email"> The email. </param>
			/// <param name="password"> The password. </param>
			public RestClient(string url, string email, string password)
			{
				_url = url;
				Login(email, password);
			}

			public RestClient(string url, string token)
			{
				Token = token;
				_url = url;
			}

			/// <summary>
			///   Gets the URL.
			/// </summary>
			private string Url
			{
				get { return _url; }
			}

			/// <summary>
			///   Gets or sets the token.
			/// </summary>
			/// <value> The token. </value>
			internal string Token
			{
				get
				{
					if (!_attemptingLogin && string.IsNullOrEmpty(_token))
						throw new NotLoggedInException("The FogBugz API is not connected");
					return _token;
				}
				set { _token = value; }
			}

			/// <summary>
			///   Logs on to the FogBugz API.
			/// </summary>
			/// <param name="username"> The username. </param>
			/// <param name="password"> The password. </param>
			private void Login(string username, string password)
			{
				try
				{
					_attemptingLogin = true;

					var doc = new XmlDocument();

					var response = Command("logon", new Dictionary<string, string> {{"email", username}, {"password", password}});
					doc.LoadXml(response);
					var tokens = doc.GetElementsByTagName("token");

					if (tokens.Count != 1)
						throw new NotLoggedInException(string.Format(CultureInfo.CurrentCulture, "Error connecting to FogBugz API: {0}", doc.InnerXml));

					Token = tokens[0].InnerText;
				}
				catch (Exception ex)
				{
					throw new NotLoggedInException(string.Format(CultureInfo.CurrentCulture, "Error connecting to the fogbugz API. Please check the username/password"), ex);
				}
				finally
				{
					_attemptingLogin = false;
				}
			}

			/// <summary>
			///   Searches using the specified query.
			/// </summary>
			/// <param name="query"> The query. </param>
			/// <param name="columns"> The columns. </param>
			/// <param name="max"> The max. </param>
			/// <returns> the search results. </returns>
			public string Search(string query, string columns = null, int? max = null)
			{
				var arguments = new Dictionary<string, string>
					{
						{"q", string.Format(CultureInfo.InvariantCulture, "{0}", query)},
						{"cols", string.Format(CultureInfo.InvariantCulture, "{0}", string.IsNullOrEmpty(columns) ? string.Join(",", DefaultColumns) : columns)},
						{"max", string.Format(CultureInfo.InvariantCulture, "{0}", max.HasValue ? (max.Value <= 0 ? 10000 : max) : 10000)}
					};
				return Command("search", arguments);
			}

			/// <summary>
			///   Sends the specified command to the REST api.
			/// </summary>
			/// <param name="command"> The command. </param>
			/// <param name="arguments"> The arguments. </param>
			/// <param name="attachemnts"> The attachemnts. </param>
			/// <returns> the result </returns>
			public string Command(string command, IDictionary<string, string> arguments = null, IEnumerable<IDictionary<string, byte[]>> attachemnts = null)
			{
				if (arguments == null) arguments = new Dictionary<string, string>();
				arguments.Add("cmd", command);
				if (!string.IsNullOrEmpty(Token))
					arguments.Add("token", Token);

				return CallRestApi(Url, arguments, attachemnts);
			}

			/// <summary>
			///   Calls the FogBugz REST API.
			/// </summary>
			/// <param name="url"> The URL. </param>
			/// <param name="arguments"> The arguments. </param>
			/// <param name="attachemnts"> The attachemnts. </param>
			/// <returns> The API call result. </returns>
			private static string CallRestApi(string url, IEnumerable<KeyValuePair<string, string>> arguments, IEnumerable<IDictionary<string, byte[]>> attachemnts)
			{
				const string newLine = "\r\n";
				const string bounds = "--------------------------------";
				const string bounds2 = "--" + bounds;

				var encoding = new ASCIIEncoding();
				var utf8Encoding = new UTF8Encoding();
				var http = (HttpWebRequest)WebRequest.Create(url);
				http.Method = "POST";
				http.AllowWriteStreamBuffering = true;
				http.ContentType = "multipart/form-data; boundary=" + bounds;

				var parts = new Queue();

				foreach (var i in arguments)
				{
					parts.Enqueue(encoding.GetBytes(bounds2 + newLine));
					parts.Enqueue(encoding.GetBytes("Content-Type: text/plain; charset=\"utf-8\"" + newLine));
					parts.Enqueue(encoding.GetBytes(string.Format(CultureInfo.InvariantCulture, "Content-Disposition: form-data; name=\"{0}\"{1}{1}", i.Key, newLine)));
					parts.Enqueue(utf8Encoding.GetBytes(i.Value));
					parts.Enqueue(encoding.GetBytes(newLine));
				}

				if (attachemnts != null)
				{
					foreach (Dictionary<string, byte[]> j in attachemnts)
					{
						parts.Enqueue(encoding.GetBytes(bounds2 + newLine));
						parts.Enqueue(encoding.GetBytes("Content-Disposition: form-data; name=\""));
						parts.Enqueue(j["name"]);
						parts.Enqueue(encoding.GetBytes("\"; filename=\""));
						parts.Enqueue(j["filename"]);
						parts.Enqueue(encoding.GetBytes("\"" + newLine));
						parts.Enqueue(encoding.GetBytes("Content-Transfer-Encoding: base64" + newLine));
						parts.Enqueue(encoding.GetBytes("Content-Type: "));
						parts.Enqueue(j["contenttype"]);
						parts.Enqueue(encoding.GetBytes(newLine + newLine));
						parts.Enqueue(j["data"]);
						parts.Enqueue(encoding.GetBytes(newLine));
					}
				}

				parts.Enqueue(encoding.GetBytes(bounds2 + "--"));

				var nContentLength = parts.Cast<byte[]>().Sum(part => part.Length);
				http.ContentLength = nContentLength;

				var stream = http.GetRequestStream();
				foreach (Byte[] part in parts)
					stream.Write(part, 0, part.Length);

				stream.Close();

				var r = http.GetResponse().GetResponseStream();
				if (r != null)
				{
					var reader = new StreamReader(r);
					var retValue = reader.ReadToEnd();
					reader.Close();
					return retValue;
				}
				throw new FogBugzException(string.Format(CultureInfo.CurrentCulture, "Error in http response stream"));
			}
		}

		#endregion
	}
}
