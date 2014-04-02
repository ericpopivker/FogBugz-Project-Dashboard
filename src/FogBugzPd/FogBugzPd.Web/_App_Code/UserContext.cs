using System;
using System.Web;
using System.Linq;
using FogBugzPd.Core;
using FogBugzPd.Infrastructure;
using FogBugzPd.Web.Utils;
using FogBugzPd.Infrastructure.Web;

namespace FogBugzPd.Web
{
	public class UserContext
	{
		protected static UserContext Current = new UserContext();

		private const string KeyIsLoggedIn = "IsLoggedIn";
		private const string KeyTimeZoneOffset = "TimeZoneOffset";
		public const string KeyFbAccountId = "FbAccountId";

		private bool IsLoggedIn_Private
		{
			get
			{
				if (HttpContext.Current.Session[KeyIsLoggedIn] == null) return false;
				return (bool)HttpContext.Current.Session[KeyIsLoggedIn];
			}
			set { HttpContext.Current.Session[KeyIsLoggedIn] = value; }
		}

		public static bool IsLoggedIn
		{
			get { return Current.IsLoggedIn_Private; }
			set { Current.IsLoggedIn_Private = value; }
		}
		
		private int? FbAccountId_Private
		{
			get
			{
				if (HttpContext.Current.Session[KeyFbAccountId] == null) return null;
				return (int)HttpContext.Current.Session[KeyFbAccountId];
			}
			set { HttpContext.Current.Session[KeyFbAccountId] = value; }
		}

	
		public static int? FbAccountId
		{
			get { return Current.FbAccountId_Private; }
			set { Current.FbAccountId_Private = value; }
		}

		
		public static string FogBugzUrl
		{
			get { return FbAccountContext.Current.Url; }
		}


		public static string FogBugzApiUrl
		{
			get
			{
				if (String.IsNullOrEmpty(FogBugzUrl))
					throw new ArgumentNullException("FogBugzUrl");
				
				string fogBugzApiUrl = UrlUtils.GenFogBugzApiUrl(FogBugzUrl);
				return fogBugzApiUrl;
			}
		}

		private int TimeZoneOffset_Private
		{
			get
			{
				if (HttpContext.Current.Session[KeyTimeZoneOffset] == null) return 0;
				return (int)HttpContext.Current.Session[KeyTimeZoneOffset];
			}
			set { HttpContext.Current.Session[KeyTimeZoneOffset] = value; }
		}

		public static int TimeZoneOffset
		{
			get { return Current.TimeZoneOffset_Private; }
			set { Current.TimeZoneOffset_Private = value; }
		}
	

		public static void ClearLoggedInUser()
		{
			IsLoggedIn = false;
			FbAccountId = null;
		}

		#region [ Notifications ]

		private string Type
		{
			get
			{
				var type = HttpContext.Current.ApplicationInstance.GetType().BaseType;
				string typeText = type != null ? type.Namespace.Split('.').Last() : "";
				return typeText;
			}
		}

		private PageNotification PageNotification_Private
		{
			get { return (PageNotification)HttpContext.Current.Session[Type + ".PageNotification"]; }
			set { HttpContext.Current.Session[Type + ".PageNotification"] = value; }
		}
		
		public static PageNotification PageNotification
		{
			get { return Current.PageNotification_Private; }
			set { Current.PageNotification_Private = value; }
		}

		public static void SetNotification(PageNotificationType type, string message)
		{
			PageNotification = new PageNotification { Type = type, Message = message };
		}

		public static void ResetNotification()
		{
			PageNotification = null;
		}

		#endregion
	}
}
