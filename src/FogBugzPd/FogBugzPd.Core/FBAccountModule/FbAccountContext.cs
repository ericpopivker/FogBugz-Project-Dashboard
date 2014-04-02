using System;
using System.Linq;
using System.Web;
using FogBugzPd.Core.FbAccountModule;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Core
{
	public class FbAccountContext
	{
		private const string Key_FbAccount = "FbAccount";

		public delegate int GetCurrentFbAccountIdDelegate();
		public static GetCurrentFbAccountIdDelegate GetContextFbAccountIdMethod;


		public static FbAccount Current
		{
			get
			{	var obj = XCallContextUtils.GetData(Key_FbAccount);
				if (obj == null)
				{
					if (GetContextFbAccountIdMethod == null)
						throw new InvalidOperationException("GetCurrentFbAccountIdMethod was not specified");

					int fbAccountId = GetContextFbAccountIdMethod();

					string cacheKey = MsCacheKey.Gen(MsCacheDataType.FbAccount, fbAccountId.ToString());

					//Try get from cache
					if (!MsCache.TryGet(cacheKey, ref obj))
					{
				
						//Get from DB
						using (var dbContextScope = new FogBugzPdDbContextScope())
						{
							obj = FogBugzPdDbContext.Current.FbAccounts
							                        .Include("Settings")
							                        .Include("Settings.TestRailConfig")
							                        .Single(fba => fba.Id == fbAccountId);

							MsCache.Set(cacheKey, obj);
						}
					}

					if (obj != null)
					{
						XCallContextUtils.SetData(Key_FbAccount, obj);
						return (FbAccount) obj;
					}
				
				}
			
				return obj as FbAccount;
			}
		}
	}
}