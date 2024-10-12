using Newtonsoft.Json;

namespace ZEA.Serializations.NewtonsoftJson.Converters;

public abstract class IntPtrJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(IntPtr value);

	protected abstract IntPtr GetValue(T instance);

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

		writer.WriteValue(GetValue(value).ToInt64());
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

		if (reader.TokenType == JsonToken.Integer)
		{
			var value = new IntPtr(Convert.ToInt64(reader.Value));
			return CreateInstance(value);
		}

		throw new JsonSerializationException($"Unexpected token parsing {typeof(T).Name}. Expected Integer, got {reader.TokenType}.");
	}
}