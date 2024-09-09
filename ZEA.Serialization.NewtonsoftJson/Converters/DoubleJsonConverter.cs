using Newtonsoft.Json;

namespace ZEA.Serialization.NewtonsoftJson.Converters;

public abstract class DoubleJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(double value);

	protected abstract double GetValue(T instance);

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
			case JsonToken.Float or JsonToken.Integer:
			{
				var value = Convert.ToDouble(reader.Value);
				return CreateInstance(value);
			}
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected Float or Integer, got {reader.TokenType}."
				);
		}
	}
}