
namespace FogBugzPd.Infrastructure.Web
{
	public enum PageNotificationType
	{
		Error = 1,
		Success,
		Progress
	}

	public class PageNotification
	{
		public PageNotificationType Type { get; set; }

		public string Message { get; set; }

		public PageNotification()
		{

		}

		public PageNotification(PageNotificationType type, string message)
		{
			Type = type;
			Message = message;
		}
	}
}
