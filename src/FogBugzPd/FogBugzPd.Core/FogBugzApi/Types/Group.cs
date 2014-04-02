using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class Group
	{
		public string Name;
		public IList<FogLampz.Model.Person> Persons;

		public Group()
		{
			Persons = new List<FogLampz.Model.Person>();
		}
	}
}
