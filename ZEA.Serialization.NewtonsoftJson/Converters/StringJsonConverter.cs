using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class StringJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(string value);

	protected abstract string GetValue(T instance);

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
				var value = reader.Value?.ToString() ?? string.Empty;
				return CreateInstance(value);
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}."
				);
		}
	}
}