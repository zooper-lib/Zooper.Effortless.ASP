using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class BoolJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(bool value);

	protected abstract bool GetValue(T instance);

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
			case JsonToken.Boolean:
				var value = Convert.ToBoolean(reader.Value);
				return CreateInstance(value);
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected Boolean, got {reader.TokenType}."
				);
		}
	}
}