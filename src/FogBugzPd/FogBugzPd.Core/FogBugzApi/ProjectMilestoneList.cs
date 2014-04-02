using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FogBugzPd.Core.ProgressStatus;
using FogLampz;
using FogLampz.Model;

namespace FogBugzPd.Core.FogBugzApi
{
	public class ProjectMilestoneList
	{
		private const int CombinationsToFind = 50;
		private const int CombinationsToProcess = 300;

		private const string ListProgressStatusKey = "ProjectListStatusKey";

		private readonly string _cacheKey;

		private string CacheKey
		{
			get { return Core.MsCacheKey.Gen(MsCacheDataType.ProjectLists, _cacheKey); }
		}

		
		#region Properties

		public List<ProjectListItem> Items { get; set; }

		public MsCacheInfo MsCache { get; set; }

		public bool ShowSubProjects { get; set; }

		#endregion


		public ProjectMilestoneList(string cacheKey)
		{
			Items = new List<ProjectListItem>();

			MsCache = new MsCacheInfo();

			_cacheKey = cacheKey;
		}

		public void GetList(IList<Project> projects, IList<FixFor> milestones, IList<Case> subProjectParentCases, int threadsCount)
		{
			milestones = milestones.Where(c => (c.EndDate() ?? DateTime.Now) >= DateTime.Today).ToList();

			var combinations = PrepareCombinations(projects, milestones);

			combinations = ProcessCombinations(combinations, threadsCount);

			var sharedMileStones = milestones.Where(milestone => milestone.IsShared()).ToList();

			var validProjects = projects.Where(p => combinations.Any(c => c.ProjectId == p.Index));

			foreach (Project project in validProjects)
			{
				var projectListItem = new ProjectListItem
				{
					Project = project
				};

				var projectId = project.Index;

				var projectMilestones = milestones.Where(m => m.IndexProject == projectId)
												  .ToList();

				projectMilestones.AddRange(sharedMileStones);

				var validMilestones = projectMilestones
					.Where(m => combinations.Any(c => c.ProjectId == projectId && c.MilestoneId == m.Index));

				foreach (var milestone in validMilestones)
				{
					var milestoneListItem = new MilestoneListItem {Milestone = milestone};

					if (subProjectParentCases != null)
						milestoneListItem.SubProjectParentCases =
							subProjectParentCases.Where(c => c.IndexProject == projectId && c.IndexFixFor == milestone.Index)
							                     .OrderBy(c => c.Title) // sort alphabetical
							                     .ToList();

					projectListItem.MilestoneListItems.Add(milestoneListItem);

				}

				Items.Add(projectListItem);
			}

			// sort projects
			// by min milestone end date. So first project will have milestone that ends the quickest. 
			Items = Items
					.OrderBy(p => p.MilestoneListItems.Min(i => i.Milestone.EndDate()))
					.ToList();

			// sort milestones
			// by end date
			Items.ForEach(item =>
			{
				item.MilestoneListItems = item.MilestoneListItems
											  .OrderBy(m => m.Milestone.EndDate())
											  .ToList();
			});

			MsCache.CreatedAt = Core.MsCache.GetObjectExpirationTime(CacheKey);
		}

		List<Combination> PrepareCombinations(IEnumerable<Project> projects, IList<FixFor> milestones)
		{
			var sharedMilestones = milestones.Where(milestone => FixForExtensions.IsShared(milestone)).ToList();

			var notSharedMilestones = milestones.Where(milestone => !FixForExtensions.IsShared(milestone)).ToList();

			var result = new List<Combination>();

			foreach (var milestone in sharedMilestones)
			{
				result.AddRange(projects.Select(p => new Combination() { ProjectId = p.Index, ProjectName = p.Name, MilestoneName = milestone.Name, MilestoneId = milestone.Index, MilestoneEndDate = milestone.EndDate() }).ToList());
			}

			foreach (var milestone in notSharedMilestones)
			{
				var project = projects.SingleOrDefault(p => p.Index == milestone.IndexProject);

				if (project != null)
					result.Add(new Combination(){ProjectId = project.Index, ProjectName = project.Name, MilestoneId = milestone.Index, MilestoneName = milestone.Name, MilestoneEndDate = milestone.EndDate()});
			}

			//var combinations = new List<Combination>();
			//foreach (var project in projects)
			//{
			//	Project prj = project;
			//	var projectMilestones = milestones.Where(m => m.IndexProject == prj.Index)
			//									  .ToList();

			//	projectMilestones.AddRange(sharedMileStones);

			//	combinations.AddRange(
			//		projectMilestones.Select(m => new Combination
			//		{
			//			MilestoneId = m.Index,
			//			ProjectId = prj.Index,
			//			MilestoneEndDate = m.EndDate(),
			//			ProjectName = prj.Name,
			//			MilestoneName = m.Name
			//		}));
			//}

			result = result.Where(c => c.MilestoneEndDate.HasValue).ToList();

			return result;
		}

		List<Combination> ProcessCombinations(List<Combination> combinations, int threadsCount)
		{
			combinations = combinations.Take(CombinationsToProcess).ToList();

			ProcessedCombinationsResult existingResult = null;
			object existingObjResult = null;

			if (Core.MsCache.TryGet(CacheKey, ref existingObjResult))
				existingResult = (ProcessedCombinationsResult)existingObjResult;

			List<Combination> existingCaseSet = null;
			if (existingResult != null && existingResult.ThreadCount == threadsCount)
			{
				existingCaseSet = existingResult.ProcessedCombinations;
			}
			else
			{
				Core.MsCache.TryRemove(CacheKey);
			}

			if (existingCaseSet != null)
				return existingCaseSet;

			// Use ConcurrentQueue to enable safe enqueueing from multiple threads. 
			var exceptions = new ConcurrentQueue<Exception>();

			var fbClient = FogBugzGateway.GetClientForParallel();

			Func<object, int> action = (object obj) =>
			{
				Combination combo = obj as Combination;
				//System.Diagnostics.Debug.WriteLine("{0} Thread: {1} Project: {2}, Milestone: {3}, StartAction",
				//										DateTime.Now.ToString("hh:mm:ss"), Thread.CurrentThread.ManagedThreadId,
				//										combo.ProjectName, combo.MilestoneName);

				
				var cases = FogBugzGateway.GetCases(combo.ProjectId.Value, combo.MilestoneId.Value, null, 1, fbClient);

				Core.MsCache.Set(ListProgressStatusKey + "_" + _cacheKey, new ProgressStatusInfo { Value = 33, Label = String.Format("Checking {0} {1} for tasks {2} of {3}", combo.ProjectName, combo.MilestoneName, combinations.IndexOf(combo) + 1, combinations.Count) });

				combo.HasCases = cases.Any(c => !c.DateClosed.HasValue);
				
				//System.Diagnostics.Debug.WriteLine("{0} Thread: {1} Project: {2}, Milestone: {3}, HasCases={4}",
				//										DateTime.Now.ToString("hh:mm:ss"), Thread.CurrentThread.ManagedThreadId, 
				//										combo.ProjectName, combo.MilestoneName, combo.HasCases);
				return 0;
			};

			var factory = new TaskFactory();

			int step = 0;

			var combinationsPerStep = threadsCount;

			while (true)
			{
				//Console.WriteLine("ProcessCominations Step " + step);
				var stepCombinations = combinations.Skip(step * combinationsPerStep).Take(combinationsPerStep).ToArray();

				if (!stepCombinations.Any()) break;

				var tasks = new Task<int>[stepCombinations.Count()];

				for (int i = 0; i < stepCombinations.Count(); i++)
				{
					tasks[i] = factory.StartNew(action, stepCombinations[i]);
				}

				//Exceptions thrown by tasks will be propagated to the main thread 
				//while it waits for the tasks. The actual exceptions will be wrapped in AggregateException. 
				
			
				Task.WaitAll(tasks);
				
				if (combinations.Count(c => c.HasCases) >= CombinationsToFind || stepCombinations.Count() < combinationsPerStep)
					break;

				step++;
			}

			if (exceptions.Any())
			{
				throw new AggregateException(exceptions);
			}

			
			// return legal combinations
			var result = combinations.Where(c => c.HasCases) //combinations with at least one task
				//.Where(c => (c.MilestoneEndDate ?? DateTime.Now) > DateTime.Now) //moved to process method
									 .OrderBy(c => c.MilestoneEndDate) //Sort by Milestone End Date in future
									 .Take(CombinationsToFind) //Get top 20 
									 .ToList();

			Core.MsCache.Set(CacheKey,
								 new ProcessedCombinationsResult { ProcessedCombinations = result, ThreadCount = threadsCount },
								 new TimeSpan(1, 0, 0));

			return result;
		}
	}
	public class ProjectListItem
	{

		public List<MilestoneListItem> MilestoneListItems { get; set; }

		public Project Project { get; set; }

		public ProjectListItem()
		{
			MilestoneListItems = new List<MilestoneListItem>();
		}
	}

	public class MilestoneListItem
	{
		public FixFor Milestone { get; set; }

		public List<Case> SubProjectParentCases { get; set; }

		public MilestoneListItem()
		{
			SubProjectParentCases = new List<Case>();
		}
	}

	public class MsCacheInfo
	{
		public DateTime CreatedAt { get; set; }

		public MsCacheDataType Type { get; set; }
	}

	public class Combination
	{
		public int? ProjectId { get; set; }
		public int? MilestoneId { get; set; }
		public bool HasCases { get; set; }
		public string CacheKey { get; set; }

		public DateTime? MilestoneEndDate { get; set; }

		public string MilestoneName { get; set; }
		public string ProjectName { get; set; }
	}

	public class ProcessedCombinationsResult
	{
		public List<Combination> ProcessedCombinations { get; set; }
		public int ThreadCount { get; set; }
	}
}
