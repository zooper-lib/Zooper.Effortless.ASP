using Newtonsoft.Json.Serialization;

namespace Zooper.Effortless.ASP.Serialization.NewtonsoftJson;

public class KnownTypesBinder : ISerializationBinder
{
	private readonly IList<Type> _knownTypes;

	public KnownTypesBinder(IList<Type> knownTypes)
	{
		_knownTypes = knownTypes;
	}

	public Type BindToType(
		string? assemblyName,
		string typeName)
	{
		return _knownTypes.SingleOrDefault(t => t.Name == typeName)!;
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