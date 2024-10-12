using Newtonsoft.Json;

namespace ZEA.Serializations.NewtonsoftJson.Converters;

public abstract class DateTimeOffsetJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(DateTimeOffset value);

	protected abstract DateTimeOffset GetValue(T instance);

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

		serializer.Serialize(writer, GetValue(value));
	}

	public override T? ReadJson(
		JsonReader reader,
		Type objectType,
		T? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
			return default;

		var dateTimeOffset = serializer.Deserialize<DateTimeOffset>(reader);
		return CreateInstance(dateTimeOffset);
	}
}