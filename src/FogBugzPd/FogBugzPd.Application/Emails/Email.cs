using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using FogBugzPd.Infrastructure;
using FogBugzPd.Infrastructure.Razor;
using FogBugzPd.Web.Utils;

namespace FogBugzPd.Application.Emails
{
	public abstract class Email<T> where T : Email<T>
	{
		protected string Template { get; set; }

		internal string Body { get; set; }

		internal bool UseMasterLayout { get; set; }

		public string Title { get; set; }

		protected Email()
			: this(false)
		{

		}

		protected Email(bool useMasterLayout, string title = null)
		{
			UseMasterLayout = useMasterLayout;
			LoadTemplate();
			Title = title;
		}

		protected void FillMessage(MailMessage msg, MailAddress to, string subject)
		{
			msg.To.Add(to);

			msg.Subject = subject;
		}

		protected void Send(MailMessage msg)
		{
			msg.IsBodyHtml = true;

			var to = msg.To.Select(t => t.Address).ToList();

			msg.Body = GetBody(to);

			XEmailUtils.SendEmailsWithSendGrid(String.Format("{0} <{1}>", msg.From.DisplayName, msg.From.Address), to, msg.Subject, msg.Body, true, null);
		}

		public string GetBody(List<string> to)
		{
			string innerBody = RazorParser.Parse((T)this, Template);

			if (UseMasterLayout)
			{
				var layout = new EmailLayout(innerBody, Title);
				//if (to != null)
					//layout.ToEmail = to;
				innerBody = layout.GetHtml();
			}

			return innerBody;
		}

		protected void LoadTemplate()
		{
			var type = this.GetType();

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\")) + "\\Emails\\Templates\\" + type.Name + ".tpl";

			using (var stream = File.OpenRead(path))
			{
				Template = new StreamReader(stream).ReadToEnd();
			}
		}
	}
}
