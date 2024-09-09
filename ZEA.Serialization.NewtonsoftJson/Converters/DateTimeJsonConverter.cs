using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class DateTimeJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(DateTime value);

	protected abstract DateTime GetValue(T instance);

	public override void WriteJson(
		JsonWriter writer,
		T? value,
		JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}

		writer.WriteValue(GetValue(value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
	}

	public override T? ReadJson(
		JsonReader reader,
		Type objectType,
		T? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer)
	{
		switch (reader.TokenType)
		{
			case JsonToken.Null:
				return default;
			case JsonToken.Date:
			{
				var value = (DateTime)reader.Value!;
				return CreateInstance(value);
			}
			case JsonToken.String:
			{
				var dateString = (string)reader.Value!;
				var value = DateTime.Parse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
				return CreateInstance(value);
			}
			case JsonToken.None:
			case JsonToken.StartObject:
			case JsonToken.StartArray:
			case JsonToken.StartConstructor:
			case JsonToken.PropertyName:
			case JsonToken.Comment:
			case JsonToken.Raw:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.Boolean:
			case JsonToken.Undefined:
			case JsonToken.EndObject:
			case JsonToken.EndArray:
			case JsonToken.EndConstructor:
			case JsonToken.Bytes:
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected Date or String, got {reader.TokenType}."
				);
		}
	}
}