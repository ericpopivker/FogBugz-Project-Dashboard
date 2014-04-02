using System;
using System.Collections.Generic;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.ProjectStatus;
using FogLampz.Model;
using NUnit.Framework;

namespace FogBugzPd.Core.Tests.ProjectStatus
{
	[TestFixture]
	public class ProjectStatusCalculatorTest
	{
		private CaseSet _caseSet;
		private List<Person> _persons;
		private Milestone _milestone;
		private bool _isScheduleLoadedByAdmin;
		private FbSchedule _schedule;

		[SetUp]
		public void Setup()
		{
			var milestoneBuilder = new MilestoneBuilder();
			var today = DateTime.Now;

			_milestone = milestoneBuilder.BuildMileStone(today.Date.AddDays(-5), today.Date.AddDays(+10), 1);
			
			var personBuilder = new PersonBuilder();

			_persons = new List<Person>(4);

			_persons.Add(personBuilder.BuildPerson("Ivan", 1));
			_persons.Add(personBuilder.BuildPerson("John", 2));
			_persons.Add(personBuilder.BuildPerson("Igor", 3));
			_persons.Add(personBuilder.BuildPerson("Sasha", 4));

			var cases = new List<Case>();

			var caseBuilder = new CaseBuilder();

			var person = _persons[0];

			cases.Add(caseBuilder.BuildCase(person, 10, 2, today.AddDays(-4), 1));
			cases.Add(caseBuilder.BuildCase(person, 10, 0, null, 2));
			cases.Add(caseBuilder.BuildCase(person, 10, 8, today.AddDays(-3), 3));

			person = _persons[1];
			cases.Add(caseBuilder.BuildCase(person, 5, 0, null, 4));
			cases.Add(caseBuilder.BuildCase(person, 5, 5, today.AddDays(-1), 5));
			cases.Add(caseBuilder.BuildCase(person, 5, 3, today, 6));

			person = _persons[2];
			cases.Add(caseBuilder.BuildCase(person, 15, 8, today.AddDays(-4), 7));
			cases.Add(caseBuilder.BuildCase(person, 15, 10, today.AddDays(-3), 8));
			cases.Add(caseBuilder.BuildCase(person, 150, 9, today.AddDays(-2), 9));

			person = _persons[3];
			cases.Add(caseBuilder.BuildCase(person, 5, 0, null, 10));
			cases.Add(caseBuilder.BuildCase(person, 5, 0, null, 11));
			cases.Add(caseBuilder.BuildCase(person, 5, 0, null, 12));


			_caseSet = new CaseSet() {Cases = cases, Milestone = _milestone};

			_isScheduleLoadedByAdmin = false;

			_schedule = new FbSchedule();
		}

		[Test]
		public void TestProjectStatusCalculator()
		{
			var projectStatusCalculator = new ProjectStatusCalculator(_caseSet, _persons, _isScheduleLoadedByAdmin, _schedule);

			var result = projectStatusCalculator.GetProjectStatus();
		}
	}

	class PersonBuilder
	{
		public Person BuildPerson(string name, int id)
		{
			return new Person() { Name = name, Index = id };
		}
	}

	class CaseBuilder
	{
		public Case BuildCase(Person assignedTo, int originalEstimate, int elapsed, DateTime? resolvedDate, int id)
		{
			var task = new Case() { IndexPersonAssignedTo = assignedTo.Index, IsOpen = true, Index = id, HoursOriginalEstimate = originalEstimate, HoursElapsed = elapsed, DateResolved = resolvedDate };

			if (resolvedDate.HasValue) task.IndexPersonResolvedBy = assignedTo.Index;

			return task;
		}
	}

	class MilestoneBuilder
	{
		public Milestone BuildMileStone(DateTime startDate, DateTime releaseDate, int id)
		{
			var fixFor = new FixFor() {Name ="Milestone 1.0", DateRelease = releaseDate, DateStart = startDate, Index = id};
			var milestone = new Milestone(fixFor) {DateRelease = releaseDate};

			return milestone;
		}
	}
}
