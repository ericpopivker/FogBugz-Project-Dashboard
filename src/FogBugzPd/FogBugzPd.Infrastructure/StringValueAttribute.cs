using System;

namespace FogBugzPd.Infrastructure
{
	[AttributeUsage(AttributeTargets.Field)]
	public class StringValueAttribute:Attribute
	{
		private string _value;

		public StringValueAttribute(string value)
		{
			_value = value;
		}

		public string GetValue()
		{
			return _value;
		}
	}
}