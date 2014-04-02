using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FogBugzPd.Core;

namespace FogBugzPd.Web.Models.Project
{
	public class MsCacheInfo
	{
		public DateTime CreatedAt { get; set; }

		public MsCacheDataType Type { get; set; }
	}
}