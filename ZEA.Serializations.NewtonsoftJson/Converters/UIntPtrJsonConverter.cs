using Newtonsoft.Json;

namespace ZEA.Serializations.NewtonsoftJson.Converters;

public abstract class UIntPtrJsonConverter<T> : JsonConverter<T>
{
	protected abstract T CreateInstance(UIntPtr value);

	protected abstract UIntPtr GetValue(T instance);

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

		writer.WriteValue(GetValue(value).ToUInt64());
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
				var value = new UIntPtr(Convert.ToUInt64(reader.Value));
				return CreateInstance(value);
			}
			default:
				throw new JsonSerializationException(
					$"Unexpected token parsing {typeof(T).Name}. Expected Integer, got {reader.TokenType}."
				);
		}
	}
}