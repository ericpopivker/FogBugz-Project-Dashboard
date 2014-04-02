using System;
using FogBugzPd.Web._App_Code.Utils;

namespace FogBugzPd.Core
{
	public class FogBugzPdDbContextScope : IDisposable
	{
		private static int OpenedCount
		{
			get
			{
				object obj = XContextDataUtils.GetData("DbContextScope.OpenedCount");
				if (obj == null)
					return 0;

				return (int)obj;
			}

			set
			{
				XContextDataUtils.SetData("DbContextScope.OpenedCount", value);
			}
		}

		public FogBugzPdDbContextScope()
		{
			if (OpenedCount == 0)
			{
				FogBugzPdDbContext.Current = new FogBugzPdDbContext();
			}

			OpenedCount++;
		}

		public FogBugzPdDbContextScope(bool isTest)
		{
			if (OpenedCount == 0)
			{
				FogBugzPdDbContext.Current = new FogBugzPdDbContext();
			}

			OpenedCount++;
		}


		#region Implementation of IDisposable

		public void Dispose()
		{
			OpenedCount--;

			if (OpenedCount == 0)
			{
				FogBugzPdDbContext.Current.Dispose();
				FogBugzPdDbContext.Current = null;
			}
		}

		#endregion
	}
}
