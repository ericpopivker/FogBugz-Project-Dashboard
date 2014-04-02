using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestRail.MiniAPI.Utils
{
	public static class ReflectionUtils
	{
		public static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
		}
	}
}
