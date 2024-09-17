using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class EnumJsonConverter<T> : JsonConverter<T> where T : Enum
{
	protected abstract T CreateInstance(string name);

	protected abstract string GetValue(T instance);

	public override void WriteJson(
		JsonWriter writer,
		T? value,
		JsonSerializer serializer)
	{
		writer.WriteValue(GetValue(value));
	}

	public override T ReadJson(
		JsonReader reader,
		Type objectType,
		T? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String)
		{
			var name = reader.Value?.ToString() ?? string.Empty;
			return CreateInstance(name);
		}

		throw new JsonSerializationException(
			$"Unexpected token parsing {typeof(T).Name}. Expected String, got {reader.TokenType}."
		);
	}
}