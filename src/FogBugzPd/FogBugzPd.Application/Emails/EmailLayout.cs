using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FogBugzPd.Web.Utils;
using FogBugzPd.Infrastructure.Razor;

namespace FogBugzPd.Application.Emails
{
	public class EmailLayout
	{
		public string MailHtml { get; set; }

		protected string Template { get; set; }

		public string ToEmail { get; set; }

		public string Title { get; set; }

		public string WebRoot { get; set; }

		public string UnsubscribeUrl { get; set; }

		public EmailLayout(string innerHtml, string title)
		{
			MailHtml = innerHtml;

			Title = title;

			var type = this.GetType();

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/", "\\")) + "\\Emails\\Templates\\" + type.Name + ".tpl";

			using (var stream = File.OpenRead(path))
			{
				Template = new StreamReader(stream).ReadToEnd();
			}
		}

		public string GetHtml()
		{
			return RazorParser.Parse(this, Template);
		}
	}
}
