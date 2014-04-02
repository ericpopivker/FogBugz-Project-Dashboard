using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FogBugzPd.Core.TestRailApi
{
	public class TestRailPlansSummary
	{
		public int PassedCount { get; set; }
		public int BlockedCount { get; set; }
		public int RetestCount { get; set; }
		public int UntestedCount { get; set; }
		public int FailedCount { get; set; }

		public int PassedEstimate { get; set; }
		public int BlockedEstimate { get; set; }
		public int RetestEstimate { get; set; }
		public int UntestedEstimate { get; set; }
		public int FailedEstimate { get; set; }

		#region | Calculated properties |

		public int TotalCount
		{
			get
			{
				var retVal = PassedCount + BlockedCount + RetestCount + UntestedCount + FailedCount;
				return retVal != 0 ? retVal : 1;
			}
		}

		public int PassedPercentage
		{
			get { return (int) Math.Round(100.0*PassedCount/TotalCount, 0); }
		}

		public int BlockedPercentage
		{
			get { return (int) Math.Round(100.0*BlockedCount/TotalCount, 0); }
		}

		public int RetestPercentage
		{
			get { return (int) Math.Round(100.0*RetestCount/TotalCount, 0); }
		}

		public int UntestedPercentage
		{
			get { return (int) Math.Round(100.0*UntestedCount/TotalCount, 0); }
		}

		public int FailedPercentage
		{
			get { return (int) Math.Round(100.0*FailedCount/TotalCount, 0); }
		}

		public int TotalEstimate
		{
			get
			{
				var retVal = PassedEstimate + BlockedEstimate + RetestEstimate + UntestedEstimate + FailedEstimate;
				return retVal != 0 ? retVal : 1;
			}
		}

		public int PassedEstimatePercentage
		{
			get { return (int) Math.Round(100.0*PassedEstimate/TotalEstimate, 0); }
		}

		public int BlockedEstimatePercentage
		{
			get { return (int) Math.Round(100.0*BlockedEstimate/TotalEstimate, 0); }
		}

		public int RetestEstimatePercentage
		{
			get { return (int) Math.Round(100.0*RetestEstimate/TotalEstimate, 0); }
		}

		public int UntestedEstimatePercentage
		{
			get { return (int) Math.Round(100.0*UntestedEstimate/TotalEstimate, 0); }
		}

		public int FailedEstimatePercentage
		{
			get { return (int) Math.Round(100.0*FailedEstimate/TotalEstimate, 0); }
		}

		#endregion
	}
}
