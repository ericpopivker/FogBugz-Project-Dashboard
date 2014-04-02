using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TestRail.MiniAPI.Utils;

namespace TestRail.MiniAPI.Json.Converters
{
	public class UnixDateTimeConverter : DateTimeConverterBase
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var isNullable = ReflectionUtils.IsNullableType(objectType);
			if (reader.TokenType == JsonToken.Null)
			{
				if (!isNullable)
				{
					throw new JsonSerializationException("Cannot deserialize NULL value to not nullable data type");
				}
				return null;
			}

			if (reader.TokenType == JsonToken.Integer)
			{
				var intValue = (long) reader.Value;
				var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
				return dt.AddSeconds(intValue);
			}

			throw new JsonSerializationException(string.Format("Cannot deserialize token of type {0}", reader.TokenType));
		}
	}
}
