using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FogBugzPd.Infrastructure;


namespace FogBugzPd.Web.Utils
{
	/// <summary>Current application environment.</summary>
	public enum EnvironmentType
	{
		Dev = 1,
		QA = 2,
		Stable = 3,
		Prod = 4
	}

	public enum TenantMode
	{
		Single = 1,
		Multiple
	}

	public class TenantConfiguration
	{
		public TenantMode Mode { get; set; }

		public string FogBugz_Username { get; set; }
		
		public string FogBugz_Password { get; set; }
		
		public string FogBugz_Url { get; set; }

		public bool LocalLogin_Enabled { get; set; }

		public string LocalLogin_Username { get; set; }

		public string LocalLogin_Password { get; set; }
	}


	public static class EnvironmentConfig
	{

		/// <summary>Retrieve string from application config file</summary>
		/// <param name="key">Key that identifies the value</param>
		/// <remarks>Use for setttings common to all environments (dev, staging, production)</remarks>
		/// <returns>String value</returns>
		public static string GetConfigSettingStr(string key)
		{
			object value = ConfigurationManager.AppSettings[key];

			string result = XConvert.ObjToText(value);

			return result;
		}

		/// <summary>Retreive int from application config file</summary>
		/// <param name="key">Key that identifies the value</param>
		/// <remarks>Use for setttings common to all environments (dev, staging, production)</remarks>
		/// <returns>Integer value</returns>
		public static int? GetConfigSettingInt(string key)
		{
			string value = GetConfigSettingStr(key);
			int? result = XConvert.TextToInt(value);
			return result;
		}

		/// <summary>Returns name of smtp server used for sending an e-mail.</summary>
		/// <returns>Full name of Smtp Server</returns>
		/// <remarks>Used in SendEmail method.</remarks>
		public static string GetSmtpServer()
		{
			string smtpServer = GetConfigSettingStr("SmtpServer");
			if (smtpServer == "")
				throw new Exception("EnvironmentUtils::GetSmtpServer  Not specified in config file.");

			List<string> vals = XStringUtils.Split(smtpServer);
			return vals[0];
		}

		/// <summary>Retreive user name and password for SMTP server.</summary>
		/// <returns>Boolena value indicating if Cretentials have been provided</returns>
		/// <remarks>Used in SendEmail method.</remarks>
		public static bool GetSmtpServerCredentials(ref string username, ref string password)
		{
			string smtpServer = GetConfigSettingStr("SmtpServer");
			if (smtpServer == "")
				throw new Exception("EnvironmentUtils::GetSmtpServerrCredentials  'SmtpServer' is not specified in config file.");

			List<string> vals = XStringUtils.Split(smtpServer);
			if (vals.Count == 1)
				return false;

			username = vals[1];
			password = vals[2];

			return true;
		}

		public static TenantConfiguration TenantConfiguration
		{
			get
			{
				var config = new TenantConfiguration
					{
						Mode = TenantMode.Single,
						FogBugz_Password = GetConfigSettingStr("FogBugz_Password"),
						FogBugz_Url = GetConfigSettingStr("FogBugz_Url"),
						FogBugz_Username = GetConfigSettingStr("FogBugz_Username"),
						LocalLogin_Enabled = true
					};

				TenantMode mode = TenantMode.Single;

				if (Enum.TryParse(GetConfigSettingStr("TenantMode"), true, out mode))
				{
					config.Mode = mode;
				}

				bool isLocalLoginEnabled = true;

				if (bool.TryParse(GetConfigSettingStr("LocalLogin_Enabled"), out isLocalLoginEnabled))
				{
					config.LocalLogin_Enabled = isLocalLoginEnabled;
				}

				return config;
			}
		}

		public static bool UseErrorLog
		{
			get { return GetConfigSettingInt("UseErrorLog") == 1; }
		}

		public static string ConnectionString
		{
			get { return GetConnectionString("ConnString", false); }
		}

		public static string ConnectionStringName
		{
			get { return GetConnectionString("ConnString", true); }
		}

		private static string GetConnectionString(string key, bool nameOnly)
		{
			//string envKey = GetEnvironmentKey() + "_" + key;

			ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[key];

			string result = null;

			if (css != null)
				result = nameOnly ? css.Name : css.ConnectionString;

			return result;

		}

		public static bool UseMiniProfiler
		{
			get { return GetConfigSettingInt("UseMiniProfiler") == 1; }
		}

		public static bool RequireHttps
		{
			get { return GetConfigSettingInt("RequireHttps") == 1; }
		}

		public static bool HideSettings
		{
			get { return GetConfigSettingInt("HideSettings") == 1; }
		}

		public static string LocalLoginUsername
		{
			get { return GetConfigSettingStr("LocalLogin_Username"); }
		}

		public static string LocalLoginPassword
		{
			get { return GetConfigSettingStr("LocalLogin_Password"); }
		}

		public static string GoogleAnalyticsCode
		{
			get { return GetConfigSettingStr("GoogleAnalyticsCode");}
		}

		public static string SnapEngageCode
		{
			get { return GetConfigSettingStr("SnapEngageCode"); }
		}

		public static EnvironmentType GetEnvType()
		{
			var environment = GetConfigSettingStr("EnvironmentType");

			switch (environment)
			{
				case "Stable":
					return EnvironmentType.Stable;
				case "Dev":
					return EnvironmentType.Dev;
				case "Prod":
					return EnvironmentType.Prod;
			}

			return EnvironmentType.Dev;
		}

		public static string GetDebugEmail()
		{
			string supportEmail = GetConfigSettingStr("DebugEmail");
			if (supportEmail == "")
				throw new Exception("EnvironmentUtils::DebugEmail  Not specified in config file.");

			return supportEmail;
		}

		public static string GetFrontEndWebRootUrl()
		{
			string webRootUrl = GetConfigSettingStr("FrontEndWebRoot");
			if (webRootUrl == "")
				throw new Exception("EnvironmentUtils::FrontEndWebRoot  Not specified in config file.");

			return webRootUrl;
		}
	}
}