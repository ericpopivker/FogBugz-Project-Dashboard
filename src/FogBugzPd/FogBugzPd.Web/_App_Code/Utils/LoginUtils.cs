using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using FogBugzPd.Application;
using FogBugzPd.Core;
using FogLampz;
using FogLampz.Attributes;
using FogLampz.Model;

namespace FogBugzPd.Web.Utils
{
	internal class CaseCreationListener : IFogBugzClientListener
	{
		public void PostInitializeEntity(IFogBugzEntity entity, IDictionary<string, string> rawValues)
		{
			if (HttpContext.Current != null && entity is Case && FbAccountContext.Current.Settings.AllowQaEstimates)
			{
				var fieldName = FbAccountContext.Current.Settings.QaEstimateCustomFieldName;
				//"plugin_kilnplugin_at_fogcreek_com_fkilnreview";

				var filteredValues = rawValues.Where(k => k.Key.ToLower().Contains(fieldName.ToLower()))
				                              .Select(v => v.Value)
				                              .ToList();

				if (fieldName != null && filteredValues.Any())
				{
					var converter = new PropertyMapAttribute(string.Empty, ConversionStrategy.Integer);

					var val = converter.Convert(filteredValues.First());
					(entity as Case).TestEstimate = (int?)val;
				}
			}
		}
	}

	public class LoginUtils
	{
		public static bool LoginToFogBugz(string fogBugzUrl, string fogBugzUsername, string fogBugzPassword)
		{
			UserContext.FbAccountId = null;
			UserContext.IsLoggedIn = false;

			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FogBugz_ClientByFogBugzUrl, fogBugzUrl);

			object existingFbClient = null;
			string fogBugzApiUrl = UrlUtils.GenFogBugzApiUrl(fogBugzUrl);

			if (!MsCache.TryGet(cacheKey, ref existingFbClient))
			{
				FogBugzClientEx fbClient = new FogBugzClientEx(){Listener = new CaseCreationListener()};
				try
				{
					//This function may throw exception which may need to be handled by calling method
					fbClient.LogOn(fogBugzApiUrl, fogBugzUsername, fogBugzPassword);
				}
				catch (Exception)
				{
					return false;
				}

				MsCache.Set(cacheKey, fbClient);
			}
			else
			{
				if (!FogBugzClientEx.IsValidLogin(fogBugzApiUrl, fogBugzUsername, fogBugzPassword))
				{
					return false;
				}
			}

			//Set context account
			var fbAccount = new FbAccountService().Ensure(fogBugzUrl);

			UserContext.FbAccountId = fbAccount.Id;
			UserContext.IsLoggedIn = true;


			return true;
		}

	}
}