using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FogBugzPd.Application.Utils;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Core.FogBugzApi.Types;
using FogBugzPd.Core.ProjectStatus;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Application.Emails
{
	public class DailyDigest : Email<DailyDigest>
	{
		public ProjectMilestoneList ProjectMilestoneList { get; set; }

		public Dictionary<ProjectStatusListItem, ProjectStatus> Statuses { get; set; }

		public bool ShowSubProjects { get; set; }

		public string Email { get; set; }

		public DailyDigest(ProjectMilestoneList projectList, Dictionary<ProjectStatusListItem, ProjectStatus> statuses, bool showSubProjects, string email):base(true)
		{
			Statuses = statuses;

			ProjectMilestoneList = projectList;

			ShowSubProjects = showSubProjects;

			Email = email;

			Title = "FogBugz Project Dashboard for " + StringUtils.FormatDateTime(DateTime.Today);
		}

		public void Send()
		{
			var msg = new MailMessage();
			string subject = "FogBugz Project Dashboard for " + StringUtils.FormatDateTime(DateTime.Today);

			var to = new MailAddress(Email);

			FillMessage(msg, to, subject);

			msg.From = new MailAddress("no-reply@fogbugzpd.com", "fogbugzpd.com");

			Send(msg);
		}
	}
}
