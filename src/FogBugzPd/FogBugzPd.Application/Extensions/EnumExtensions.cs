using System;
using FogBugzPd.Infrastructure;

namespace FogBugzPd.Application.Extensions
{
	public static class EnumExtensions
	{
		public static string GetStringValue<T>(this T value) where T : struct,IConvertible
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type"); ;
			// Get the type
			var type = value.GetType();
			// Get fieldinfo for this type
			var fieldInfo = type.GetField(value.ToString());

			// Get the stringvalue attributes
			var attribs = fieldInfo.GetCustomAttributes(
				typeof(StringValueAttribute), false) as StringValueAttribute[];

			// Return the first if there was a match.
			return attribs != null && attribs.Length > 0 ? attribs[0].GetValue() : fieldInfo.Name;
		}
	}
}