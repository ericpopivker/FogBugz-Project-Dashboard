using System;
using Newtonsoft.Json;
using TestRail.MiniAPI.Utils;

namespace TestRail.MiniAPI.Json.Converters
{
	public class BoolConverter : JsonConverter
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

				if (intValue == 0) return false;
				if (intValue == 1) return true;
				throw new JsonSerializationException(string.Format("0 or 1 expected as bool value, but got {0} instead", intValue));
			}

			throw new JsonSerializationException(string.Format("Cannot deserialize token of type {0}", reader.TokenType));
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof (bool) || objectType == typeof (bool?);
		}
	}
}
