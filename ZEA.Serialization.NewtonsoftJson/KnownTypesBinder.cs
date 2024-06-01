using Newtonsoft.Json.Serialization;

namespace ZEA.Serialization.NewtonsoftJson;

public class KnownTypesBinder(IList<Type> knownTypes) : ISerializationBinder
{
	public Type BindToType(
		string? assemblyName,
		string typeName)
	{
		return knownTypes.SingleOrDefault(t => t.Name == typeName)!;
	}

	public void BindToName(
		Type serializedType,
		out string? assemblyName,
		out string? typeName)
	{
		assemblyName = null;
		typeName = serializedType.Name;
	}
}