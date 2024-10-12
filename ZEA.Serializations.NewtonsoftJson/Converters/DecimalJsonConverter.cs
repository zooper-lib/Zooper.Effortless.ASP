using Newtonsoft.Json;

namespace ZEA.Serializations.NewtonsoftJson.Converters;

public abstract class DecimalJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(decimal value);

	protected abstract decimal GetValue(T instance);

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
			case JsonToken.Float:
			case JsonToken.Integer:
			case JsonToken.String:
				var value = Convert.ToDecimal(reader.Value);
				return CreateInstance(value);
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected Float, Integer, or String, got {reader.TokenType}."
				);
		}
	}
}