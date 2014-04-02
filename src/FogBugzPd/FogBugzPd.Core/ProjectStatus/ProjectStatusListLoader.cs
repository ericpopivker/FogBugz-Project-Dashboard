using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FogBugzPd.Core;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.ProgressStatus;
using FogBugzPd.Core.ProjectStatus;

namespace FogBugzPd.Core.ProjectStatus
{
	public class ProjectStatusListEventArgs: EventArgs
	{
		public ProjectStatusListItem ProjectMilestone { get; set; }

		public ProjectStatus Status { get; set; }

		public string CacheKey { get; set; }

		public ProjectStatusListEventArgs(ProjectStatusListItem projectMilestone, ProjectStatus value, string key)
		{
			ProjectMilestone = projectMilestone;
			Status = value;
			CacheKey = key;
		}
	}

	public delegate void SaveProjectMilestoneStatus(object sender, ProjectStatusListEventArgs e);

	
	public class ProjectStatusListLoader
	{
		public event SaveProjectMilestoneStatus SaveStatusEvent;

		private const string ListProgressStatusKey = "ProjectListStatusKey";

		protected virtual void SaveStatus(ProjectStatusListEventArgs e)
		{
			if (SaveStatusEvent != null) SaveStatusEvent(this, e);
		}

		private string _cacheKey;

		public ProjectStatusListLoader(string cacheKey)
		{
			_cacheKey = cacheKey;
		}

		public string BeginGettingProjectStatuses(List<ProjectListItem> combinations)
		{
			var fbClient = FogBugzGateway.GetClientForParallel();
			
			var persons = fbClient.GetPersons().ToList();

			var milestones = GetOnlyStartedMilstones(combinations);

			var isLoadedByAdmin = FogBugzGateway.GetIsScheduleLoadedByAdmin();
			var fbSchedule = FogBugzGateway.GetFbSchedule();

			Func<object, int> action = (object obj) =>
			{
				ProjectStatusListItem combo = obj as ProjectStatusListItem;

				Core.MsCache.Set(ListProgressStatusKey + "_" + _cacheKey, new ProgressStatusInfo { Value = 66, Label = String.Format("Calculating status for \"{0} {1}\" for tasks {2} of {3}", combo.Project.Name, combo.Milestone.Name, milestones.IndexOf(combo) + 1, milestones.Count) });

				var cases = FogBugzGateway.GetCases(combo.ProjectId, combo.MileStoneId, combo.SubProjectParentCaseId, null, fbClient);

				var caseSet = new CaseSet();
				caseSet.Cases = cases;
				caseSet.Milestone = new Milestone(combo.Milestone);
				caseSet.Project =  new Project(combo.Project);

				var projectStatusCalculator = new ProjectStatusCalculator(caseSet, persons, isLoadedByAdmin, fbSchedule);

				var status = projectStatusCalculator.GetProjectStatus();

				var eventArgs = new ProjectStatusListEventArgs(combo, status, _cacheKey);

				SaveStatus(eventArgs);

				return 0;
			};

			var factory = new TaskFactory();

			var tasks = new Task<int>[milestones.Count()];

			for (int i = 0; i < milestones.Count(); i++)
			{
				tasks[i] = factory.StartNew(action, milestones[i]);
			}

			return GenKeysString(milestones);
		}

		public Dictionary<ProjectStatusListItem, ProjectStatus> GetProjectStatuses(List<ProjectListItem> combinations)
		{
			var fbClient = FogBugzGateway.GetClientForParallel();

			var persons = fbClient.GetPersons().ToList();

			var milestones = GetOnlyStartedMilstones(combinations);

			var result = new Dictionary<ProjectStatusListItem, ProjectStatus>();
			
			var isLoadedByAdmin = FogBugzGateway.GetIsScheduleLoadedByAdmin();
			var fbSchedule = FogBugzGateway.GetFbSchedule();

			Func<object, int> action = (object obj) =>
			{
				ProjectStatusListItem combo = obj as ProjectStatusListItem;

				Core.MsCache.Set(ListProgressStatusKey + "_" + _cacheKey, new ProgressStatusInfo { Value = 66, Label = String.Format("Calculating status for \"{0} {1}\" for tasks {2} of {3}", combo.Project.Name, combo.Milestone.Name, milestones.IndexOf(combo) + 1, milestones.Count) });

				var cases = FogBugzGateway.GetCases(combo.ProjectId, combo.MileStoneId, combo.SubProjectParentCaseId, null, fbClient);

				var caseSet = new CaseSet();
				caseSet.Cases = cases;
				caseSet.Milestone = new Milestone(combo.Milestone);
				caseSet.Project = new Project(combo.Project);

				var projectStatusCalculator = new ProjectStatusCalculator(caseSet, persons, isLoadedByAdmin, fbSchedule);

				var status = projectStatusCalculator.GetProjectStatus();

				result.Add(combo, status);

				return 0;
			};

			var factory = new TaskFactory();

			var tasks = new Task<int>[milestones.Count()];

			for (int i = 0; i < milestones.Count(); i++)
			{
				tasks[i] = factory.StartNew(action, milestones[i]);
			}

			Task.WaitAll(tasks);

			return result;
		}

		private string GenKeysString(List<ProjectStatusListItem> milestones)
		{
			var sb = new StringBuilder();
			
			foreach (var milestone in milestones)
			{
				sb.Append(milestone.Key);
				sb.Append(",");
			}
			return sb.ToString().Trim(',');
		}

		private List<ProjectStatusListItem> GetOnlyStartedMilstones(List<ProjectListItem> combinations)
		{
			var milestones = new List<ProjectStatusListItem>();

			foreach (var item in combinations)
			{
				foreach (var milestone in item.MilestoneListItems)
				{
					if (milestone.SubProjectParentCases != null && milestone.SubProjectParentCases.Count > 0)
					{
						foreach (var projectParentCase in milestone.SubProjectParentCases)
						{
							milestones.Add(new ProjectStatusListItem()
								{
									ProjectId = item.Project.Index.Value,
									Project = item.Project,
									MileStoneId = milestone.Milestone.Index.Value,
									Milestone = milestone.Milestone,
									SubProjectParentCaseId = projectParentCase.Index,
									SubProjectParentCase = projectParentCase
								});
						}
					}

					milestones.Add(new ProjectStatusListItem()
					{
						ProjectId = item.Project.Index.Value,
						Project = item.Project,
						MileStoneId = milestone.Milestone.Index.Value,
						Milestone = milestone.Milestone,
						SubProjectParentCaseId = null,
						SubProjectParentCase = null
					});
					
				}
			}

			return milestones;
		}
	}
}