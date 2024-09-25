using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class LongJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(long value);

	protected abstract long GetValue(T instance);

	public override void WriteJson(
		JsonWriter writer,
		T? value,
		JsonSerializer serializer)
	{
		if (value is null)
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
			case JsonToken.Integer:
			{
				var value = Convert.ToInt64(reader.Value);
				return CreateInstance(value);
			}
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected Integer, got {reader.TokenType}."
				);
		}
	}
}