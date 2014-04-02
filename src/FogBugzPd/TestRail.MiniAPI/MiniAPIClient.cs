using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using TestRail.MiniAPI.Communication;
using TestRail.MiniAPI.Entities;

namespace TestRail.MiniAPI
{
	public class MiniAPIClient
	{
		public MiniAPIClient(string url, string apiKey)
		{
			ApiKey = apiKey;
			Url = url;
		}

		public string Url { get; private set; }
		public string ApiKey { get; private set; }

		#region | Projects |

		public List<Project> GetProjects()
		{
			var request = CreateGetProjectsRequest();
			var response = MakeRawRequest(request);

			var projectsReponse = JsonConvert.DeserializeObject<GetProjectsResponse>(response);

			if (projectsReponse.Result) return projectsReponse.Projects;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", projectsReponse.Error));
		}

		public Project GetProject(int projectId)
		{
			var request = CreateGetProjectRequest(projectId);
			var response = MakeRawRequest(request);

			var projectResponse = JsonConvert.DeserializeObject<GetProjectResponse>(response);

			if (projectResponse.Result) return projectResponse.Project;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", projectResponse.Error));
		}

		#endregion

		#region | Milestones |

		public List<Milestone> GetMilestones(int projectId)
		{
			var request = CreateGetMilestonesRequest(projectId);
			var response = MakeRawRequest(request);

			var milestonesReponse = JsonConvert.DeserializeObject<GetMilestonesResponse>(response);

			if (milestonesReponse.Result) return milestonesReponse.Milestones;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", milestonesReponse.Error));
		}

		public Milestone GetMilestone(int milestoneId)
		{
			var request = CreateGetMilestoneRequest(milestoneId);
			var response = MakeRawRequest(request);

			var milestoneResponse = JsonConvert.DeserializeObject<GetMilestoneResponse>(response);

			if (milestoneResponse.Result) return milestoneResponse.Milestone;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", milestoneResponse.Error));
		}

		#endregion

		#region | Plans |

		public List<Plan> GetPlans(int projectId)
		{
			var request = CreateGetPlansRequest(projectId);
			var response = MakeRawRequest(request);

			var plansReponse = JsonConvert.DeserializeObject<GetPlansResponse>(response);

			if (plansReponse.Result) return plansReponse.Plans;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", plansReponse.Error));
		}

		public Plan GetPlan(int planId)
		{
			var request = CreateGetPlanRequest(planId);
			var response = MakeRawRequest(request);

			var planResponse = JsonConvert.DeserializeObject<GetPlanResponse>(response);

			if (planResponse.Result) return planResponse.Plan;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", planResponse.Error));
		}
		
		#endregion

		#region | Cases |

		public List<Case> GetCases(int suiteId, int? sectionId = null)
		{
			var request = CreateGetCasesRequest(suiteId, sectionId);
			var response = MakeRawRequest(request);

			var casesResponse = JsonConvert.DeserializeObject<GetCasesResponse>(response);

			if (casesResponse.Result) return casesResponse.Cases;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", casesResponse.Error));
		}

		public Case GetCase(int caseId)
		{
			var request = CreateGetCaseRequest(caseId);
			var response = MakeRawRequest(request);

			var caseResponse = JsonConvert.DeserializeObject<GetCaseResponse>(response);

			if (caseResponse.Result) return caseResponse.Case;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", caseResponse.Error));
		}

		#endregion

		#region | Runs |

		public List<Run> GetRuns(int projectId, int? planId = null)
		{
			var request = CreateGetRunsRequest(projectId, planId);
			var response = MakeRawRequest(request);

			var runsResponse = JsonConvert.DeserializeObject<GetRunsResponse>(response);

			if (runsResponse.Result) return runsResponse.Runs;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", runsResponse.Error));
		}

		public Run GetRun(int runId)
		{
			var request = CreateGetRunRequest(runId);
			var response = MakeRawRequest(request);

			var runResponse = JsonConvert.DeserializeObject<GetRunResponse>(response);

			if (runResponse.Result) return runResponse.Run;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", runResponse.Error));
		}

		#endregion

		#region | Tests |

		public List<Test> GetTests(int runId)
		{
			var request = CreateGetTestsRequest(runId);
			var response = MakeRawRequest(request);

			var testsResponse = JsonConvert.DeserializeObject<GetTestsResponse>(response);

			if (testsResponse.Result) return testsResponse.Tests;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", testsResponse.Error));
		}

		public Test GetTest(int testId)
		{
			var request = CreateGetTestRequest(testId);
			var response = MakeRawRequest(request);

			var testResponse = JsonConvert.DeserializeObject<GetTestResponse>(response);

			if (testResponse.Result) return testResponse.Test;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", testResponse.Error));
		}

		#endregion

		#region | Suites |

		public List<Suite> GetSuites(int projectId)
		{
			var request = CreateGetSuitesRequest(projectId);
			var response = MakeRawRequest(request);

			var suitesResponse = JsonConvert.DeserializeObject<GetSuitesResponse>(response);

			if (suitesResponse.Result) return suitesResponse.Suites;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", suitesResponse.Error));
		}

		public Suite GetSuite(int suiteId)
		{
			var request = CreateGetSuiteRequest(suiteId);
			var response = MakeRawRequest(request);

			var suiteResponse = JsonConvert.DeserializeObject<GetSuiteResponse>(response);

			if (suiteResponse.Result) return suiteResponse.Suite;
			throw new InvalidOperationException(string.Format("Error occurred during request: {0}", suiteResponse.Error));
		}

		#endregion

		#region | Private methods |

		private string CreateGetProjectsRequest()
		{
			var urlRequest = string.Format(@"{0}/get_projects&key={1}", Url, ApiKey);
			return urlRequest;
		}

		private string CreateGetProjectRequest(int projectId)
		{
			var urlRequest = string.Format(@"{0}/get_project/{1}&key={2}", Url, projectId, ApiKey);
			return urlRequest;
		}

		private string CreateGetMilestonesRequest(int projectId)
		{
			var urlRequest = string.Format(@"{0}/get_milestones/{1}&key={2}", Url, projectId, ApiKey);
			return urlRequest;
		}

		private string CreateGetMilestoneRequest(int milestoneId)
		{
			var urlRequest = string.Format(@"{0}/get_milestone/{1}&key={2}", Url, milestoneId, ApiKey);
			return urlRequest;
		}

		private string CreateGetPlansRequest(int projectId)
		{
			var urlRequest = string.Format(@"{0}/get_plans/{1}&key={2}", Url, projectId, ApiKey);
			return urlRequest;
		}

		private string CreateGetPlanRequest(int planId)
		{
			var urlRequest = string.Format(@"{0}/get_plan/{1}&key={2}", Url, planId, ApiKey);
			return urlRequest;
		}

		private string CreateGetCasesRequest(int suiteId, int? sectionId)
		{
			var urlRequest = sectionId.HasValue
				                 ? string.Format(@"{0}/get_cases/{1}/{2}&key={3}", Url, suiteId, sectionId, ApiKey)
				                 : string.Format(@"{0}/get_cases/{1}&key={2}", Url, suiteId, ApiKey);
			return urlRequest;
		}

		private string CreateGetCaseRequest(int caseId)
		{
			var urlRequest = string.Format(@"{0}/get_case/{1}&key={2}", Url, caseId, ApiKey);
			return urlRequest;
		}

		private string CreateGetRunsRequest(int projectId, int? planId)
		{
			var urlRequest = planId.HasValue
								 ? string.Format(@"{0}/get_runs/{1}/{2}&key={3}", Url, projectId, planId, ApiKey)
								 : string.Format(@"{0}/get_runs/{1}&key={2}", Url, projectId, ApiKey);
			return urlRequest;
		}

		private string CreateGetRunRequest(int runId)
		{
			var urlRequest = string.Format(@"{0}/get_run/{1}&key={2}", Url, runId, ApiKey);
			return urlRequest;
		}

		private string CreateGetTestsRequest(int runId)
		{
			var urlRequest = string.Format(@"{0}/get_tests/{1}&key={2}", Url, runId, ApiKey);
			return urlRequest;
		}

		private string CreateGetTestRequest(int testId)
		{
			var urlRequest = string.Format(@"{0}/get_test/{1}&key={2}", Url, testId, ApiKey);
			return urlRequest;
		}

		private string CreateGetSuitesRequest(int projectId)
		{
			var urlRequest = string.Format(@"{0}/get_suites/{1}&key={2}", Url, projectId, ApiKey);
			return urlRequest;
		}

		private string CreateGetSuiteRequest(int suiteId)
		{
			var urlRequest = string.Format(@"{0}/get_suite/{1}&key={2}", Url, suiteId, ApiKey);
			return urlRequest;
		}

		private static string MakeRawRequest(string request)
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(request);
				webRequest.Accept = "application/json";

				webRequest.Method = "GET";

				var response = (HttpWebResponse)webRequest.GetResponse();

				if (response.StatusCode != HttpStatusCode.OK) throw new Exception(string.Format("Server error (HTTP {0}: {1})", response.StatusCode, response.StatusDescription));

				return new StreamReader(response.GetResponseStream()).ReadToEnd();
			}
			catch (Exception)
			{
				return null;
			}
		}
	
		#endregion
	}
}
