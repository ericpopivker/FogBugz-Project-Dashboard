
using System;
using System.Linq;
using FogBugzPd.Core;
using FogBugzPd.Core.FbAccountModule;
using FogBugzPd.Core.FogBugzApi;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Application
{
	public class FbAccountService : ServiceBase
	{
		public FbAccount Ensure(string fbAccountUrl)
		{
			Verify.Argument.IsNotNull(fbAccountUrl, "fbAccountUrl");

			FbAccount fbAccount = null;
			if (!DbContext.FbAccounts.Any(u => u.Url == fbAccountUrl))
			{
				fbAccount = new FbAccount { Url = fbAccountUrl };
				DbContext.FbAccounts.Add(fbAccount);

				DbContext.SaveChanges();
			}
			else
			{
				fbAccount = DbContext.FbAccounts
				                     .Include("Settings")
									 .Include("Settings.TestRailConfig")
									 .Single(u => u.Url == fbAccountUrl);
			}

			return fbAccount;
		}

		public void Update(FbAccount account)
		{
			Verify.Argument.IsNotNull(account, "account");
			Verify.Argument.IsNotNull(account.Id, "account.Id");

			if (account.Settings != null && !account.Settings.AllowTestRail && account.Settings.TestRailConfig != null)
			{
				var ent = account.Settings.TestRailConfig;
				account.Settings.TestRailConfig = null;
				DbContext.TestRailConfigurations.Remove(ent);
			}

			DbContext.SaveChanges();
			
			//Update in cache
			string cacheKey = MsCacheKey.Gen(MsCacheDataType.FbAccount, account.Id.ToString());
			MsCache.TryRemove(cacheKey);
		}


	}
}
