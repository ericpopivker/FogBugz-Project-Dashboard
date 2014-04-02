using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using FogLampz.Exceptions;
using FogLampz.Model;
using FogLampz.Util;
using StackExchange.Profiling;


namespace FogLampz
{

	public interface IFogBugzClientListener
	{
		void PostInitializeEntity(IFogBugzEntity entity, IDictionary<string, string> rawValues);
	}

	//Copy of FogBugzClient, except
	//Not static, no caching
	//Much more lightweight
	public class FogBugzClientEx
	{
		public IFogBugzClientListener Listener { private get; set; }

		private RestClient Client { get; set; }

		public static bool IsValidLogin(string url, string email, string password)
		{
			var uri = new Uri(url);
			
			try
			{
				var client = new RestClient(uri.AbsoluteUri, email, password);
			}
			catch
			{
				return false;
			}

			return true;
		}


		public void LogOn(string url, string email, string password)
		{
			LogOn(new Uri(url), email, password);
		}
		
		
		public void LogOn(Uri uri, string email, string password)
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
		
		public void LogOn(string url, string token)
		{
			Client = new RestClient(url, token);
		}

		public string GetToken()
		{
			if (Client == null) return null;
			return Client.Token;
		}

		public IEnumerable<Filter> GetFilters()
		{
			var list = new List<Filter>();
			return list;
		}

		public void SetFilter(int filter)
		{
		}

		public Case GetCase(int index)
		{
			return GetCases(new[] {index}).FirstOrDefault();
		}

		public IEnumerable<Case> GetCases(IEnumerable<int> caseIndexes)
		{
			return Search<Case>(string.Join(",", caseIndexes));
		}

		public IEnumerable<Case> GetCases(string q, int? max = null)
		{
			return Search<Case>(q, null, max);
		}

		public IEnumerable<Case> GetCases(string q, string cols)
		{
			return Search<Case>(q, cols);
		}

		public Case CreateCase(Case @case)
		{
			return Create(@case);
		}


		public IEnumerable<Category> GetCategories()
		{
			var list = List<Category>();
			return list;
		}


		public IEnumerable<Person> GetPersons()
		{
			var args = new Dictionary<string, string>();
			args.Add("fIncludeActive", "1");
			args.Add("fIncludeDeleted","1");

			var list = List<Person>(args);
			list.Insert(0, Person.ClosedUser);

			return list;
		}

		public FixFor CreateFixFor(FixFor fixFor)
		{
			return Create(fixFor);
		}


		public IEnumerable<FixFor> GetFixFors(int projectId=0)
		{
			var args = new Dictionary<string, string>();
			args.Add("fIncludeDeleted", "0");
			args.Add("fIncludeReallyDeleted", "0");

			return List<FixFor>(args);
		}


		public Area CreateArea(Area area)
		{
			return Create(area);
		}


		public IEnumerable<Area> GetAreas()
		{
			var list = List<Area>();
			return list;
		}

		public IEnumerable<Priority> GetPriorities()
		{
			var list = List<Priority>();
			return list;
		}

		public Project CreateProject(Project project)
		{
			return Create(project);
		}

		public IEnumerable<Project> GetProjects()
		{
			var list = List<Project>();
			return list;
		}

		public IEnumerable<Status> GetStatuses()
		{
			var list = List<Status>();
			return list;
		}

		public IEnumerable<Mailbox> GetMailboxes()
		{
			return List<Mailbox>();
		}


		private TEntity Create<TEntity>(TEntity entity)
			where TEntity : IFogBugzEntity, new()
		{
			if (string.IsNullOrEmpty(entity.ApiInfo.CreateCommand))
				throw new InvalidOperationException("Entity does not support the create operation");

			var res = Client.Command(entity.ApiInfo.CreateCommand, entity.GetPropertyValues());
			var reader = res.GetXmlReader();
			return ParseEntityXml<TEntity>(reader, entity.ApiInfo.Root, entity.ApiInfo.Element).FirstOrDefault();
		}

		private  IList<TEntity> List<TEntity>(IDictionary<string,string> arguments = null)
			where TEntity : IFogBugzEntity, new()
		{
			using (MiniProfiler.Current.Step("Fogbugz API (list query)"))
			{
				var apiInfo = new TEntity().ApiInfo;
				var response = Client.Command(apiInfo.ListCommand, arguments);
				using (var stringReader = new StringReader(response.Trim()))
				{
					using (var xmlReader = XmlReader.Create(stringReader))
					{
						xmlReader.MoveToContent();
						return ParseEntityXml<TEntity>(xmlReader, apiInfo.Root, apiInfo.Element).ToList();
					}
				}
			}
		}


		private IList<TEntity> Search<TEntity>(string query, string cols = null, int? max = null)
			where TEntity : IFogBugzEntity, new()
		{
			using (MiniProfiler.Current.Step("Fogbugz API (search query)"))
			{
				var apiInfo = new TEntity().ApiInfo;

				var response = Client.Search(query, cols, max);

				using (var stringReader = new StringReader(response.Trim()))
				{
					using (var xmlReader = XmlReader.Create(stringReader))
					{
						xmlReader.MoveToContent();
						return ParseEntityXml<TEntity>(xmlReader, apiInfo.Root, apiInfo.Element).ToList();
					}
				}
			}
		}

		private IEnumerable<TEntity> ParseEntityXml<TEntity>(XmlReader reader, string root, string element)
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

					if (Listener != null)
					{
						Listener.PostInitializeEntity(entity, fields);
					}

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

				var http = (HttpWebRequest) WebRequest.Create(url);
				
				http.Method = "POST";
				http.AllowWriteStreamBuffering = true;
				http.ContentType = "multipart/form-data; boundary=" + bounds;

				var nContentLength = parts.Cast<byte[]>().Sum(part => part.Length);
				http.ContentLength = nContentLength;

				
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();


				//Sync
				//using (var stream = http.GetRequestStream())
				//{
				//	foreach (Byte[] part in parts)
				//		stream.Write(part, 0, part.Length);
				//}
				


				//Async
				var tempBytes = parts.Cast<byte[]>().SelectMany(p => p).ToArray();
				ManualResetEvent allDone = new ManualResetEvent(false);

				http.BeginGetRequestStream(GetRequestStreamCallback,
					new AsyncStateObject { Request = http, PostData = tempBytes, PostDataLength = tempBytes.Length, AllDone = allDone });

				//Console.WriteLine("http.ServicePoint.ConnectionLimit: " + http.ServicePoint.ConnectionLimit);

				allDone.WaitOne();

				stopwatch.Stop();

				//Console.WriteLine("http.BeginGetRequestStream Thread: " + Thread.CurrentThread.ManagedThreadId + " Duration: " + stopwatch.ElapsedMilliseconds);

				var r = http.GetResponse().GetResponseStream();
				if (r == null)
					throw new FogBugzException(string.Format(CultureInfo.CurrentCulture, "Error in http response stream"));

				
				using (var reader = new StreamReader(r))
				{
					var retValue = reader.ReadToEnd();
					return retValue;
				}
			}


			private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
			{
				var state = (AsyncStateObject)asynchronousResult.AsyncState;
				
				HttpWebRequest request = state.Request;

				// End the operation
				Stream postStream = request.EndGetRequestStream(asynchronousResult);

				postStream.Write(state.PostData, 0, state.PostData.Length);
				postStream.Close();

				state.AllDone.Set();
			}
		}

		class AsyncStateObject
		{
			public HttpWebRequest Request { get; set; }

			public byte[] PostData { get; set; }

			public int PostDataLength { get; set; }

			public ManualResetEvent AllDone { get; set; }
		}

		#endregion
	}
}
