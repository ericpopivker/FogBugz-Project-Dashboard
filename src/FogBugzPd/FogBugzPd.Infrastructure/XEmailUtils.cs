using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using FogBugzPd.Web.Utils;
using SendGrid.Transport;
using SendGrid;

namespace FogBugzPd.Infrastructure
{

	public static class XEmailUtils
	{
		public static void SendEmailsWithSendGrid(
			string from, List<string> to, string subject, string body, bool isHtml, Dictionary<string, string> uniqueIdentifiers,
			MailAddress[] replyTo = null, IEnumerable<Attachment> attachments = null)
		{
			//if (EnvironmentConfig.GetEnvType() != EnvironmentType.Prod || to.First() != "check-auth@verifier.port25.com")
			//{
			//	string newSubject = "TEST EMAIL - TO: " + string.Join(";", to);

			//	from = EnvironmentConfig.GetConfigSettingStr("FromSystemEmail");
			//	to.Clear();
			//	to.Add(EnvironmentConfig.GetDebugEmail());

			//	newSubject += "  SUBJECT: " + subject;

			//	subject = newSubject;
			//}

			// Create the email object first, then add the properties.
			Mail message = Mail.GetInstance();
			//This address is not used, but it's necessary to avoid exception
			message.AddTo("engagegrid@yahoo.com");
			message.Header.AddTo(to);
			message.From = new MailAddress(from);
			message.Subject = subject;
			if (isHtml)
			{
				message.Html = body;
			}
			else
			{
				message.Text = body;
			}

			if (uniqueIdentifiers != null)
				message.AddUniqueIdentifiers(uniqueIdentifiers);

			if (replyTo != null)
				message.ReplyTo = replyTo;

			if (attachments != null)
				foreach (var attachment in attachments)
				{
					message.AddAttachment(attachment.ContentStream, attachment.Name);
				}

			// true indicates that links in plain text portions of the email 
			// should also be overwritten for link tracking purposes. 
			message.EnableClickTracking(true);
			message.EnableOpenTracking();

			string username = null;
			string password = null;
			EnvironmentConfig.GetSmtpServerCredentials(ref username, ref password);

			// Create credentials, specifying your user name and password.
			var credentials = new NetworkCredential(username, password);

			// Create an SMTP transport for sending email.
			var transportSMTP = SMTP.GetInstance(credentials);

			// Send the email.
			transportSMTP.Deliver(message);
		}

	}
}
