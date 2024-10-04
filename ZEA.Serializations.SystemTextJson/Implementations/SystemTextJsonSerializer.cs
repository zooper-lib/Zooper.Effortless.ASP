using System.Text.Json;
using ZEA.Serializations.Abstractions.Interfaces;

namespace ZEA.Serializations.SystemTextJson.Implementations;

public class SystemTextJsonSerializer(JsonSerializerOptions options) : IJsonSerializer
{
	public string Serialize<T>(T obj)
	{
		return JsonSerializer.Serialize(obj, options);
	}

	public T Deserialize<T>(string json)
	{
		return JsonSerializer.Deserialize<T>(json, options)!;
	}
}