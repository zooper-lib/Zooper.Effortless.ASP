using Newtonsoft.Json;
using ZEA.Serializations.Abstractions.Interfaces;

namespace ZEA.Serializations.NewtonsoftJson.Implementations;

public class NewtonsoftJsonSerializer(JsonSerializerSettings settings) : IJsonSerializer
{
	public string Serialize<T>(T obj)
	{
		return JsonConvert.SerializeObject(obj, settings);
	}

	public T Deserialize<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json, settings)!;
	}
}