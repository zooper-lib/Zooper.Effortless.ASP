using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class CharJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(char value);

	protected abstract char GetValue(T instance);

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

		writer.WriteValue(GetValue(value));
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
			case JsonToken.String:
				var stringValue = reader.Value?.ToString();

				if (stringValue is { Length: 1 })
				{
					return CreateInstance(stringValue[0]);
				}

				throw new JsonSerializationException($"Expected a single character string for {typeof(T).Name}, got '{stringValue}'.");
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}."
				);
		}
	}
}