using System.Collections.Generic;

namespace FogBugzPd.Core.FogBugzApi.Types
{
	public class Tag
	{
		public string Name { get; set; }
		public int Rank { get; set; }

		public class EquantityComparer : IEqualityComparer<Tag>
		{
			public bool Equals(Tag x, Tag y)
			{
				return x.Name.Equals(y.Name);
			}

			public int GetHashCode(Tag obj)
			{
				return obj.Name.GetHashCode();
			}
		}
	}
}