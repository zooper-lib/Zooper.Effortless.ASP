using Newtonsoft.Json.Serialization;

namespace ZEA.Serialization.NewtonsoftJson.Binders;

/// <summary>
/// Serialization binder that maps type names to their corresponding types.
/// This is useful for scenarios where you need to serialize and deserialize objects
/// without tightly coupling them to specific assembly names.
/// </summary>
public sealed class TypeSerializationBinder<TType> : ISerializationBinder
{
	private readonly Dictionary<string, Type> _typeCache;

	/// <summary>
	/// Initializes a new instance of the <see cref="TypeSerializationBinder{TType}"/> class.
	/// It caches all types in the current application domain that implement <see cref="TType"/>
	/// and are not interfaces or abstract classes.
	/// </summary>
	public TypeSerializationBinder()
	{
		_typeCache = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => typeof(TType).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
			.ToDictionary(type => type.Name, type => type);
	}

	/// <summary>
	/// Binds a type name to a <see cref="Type"/> object.
	/// </summary>
	/// <param name="assemblyName">The name of the assembly. This parameter is ignored in this implementation.</param>
	/// <param name="typeName">The name of the type.</param>
	/// <returns>The <see cref="Type"/> associated with the specified type name.</returns>
	/// <exception cref="TypeLoadException">Thrown if the type name cannot be found in the cache.</exception>
	public Type BindToType(
		string? assemblyName,
		string typeName)
	{
		if (_typeCache.TryGetValue(typeName, out var type))
		{
			return type;
		}

		throw new TypeLoadException($"Could not load type {typeName}");
	}

	/// <summary>
	/// Binds a <see cref="Type"/> object to its name.
	/// </summary>
	/// <param name="serializedType">The type to be serialized.</param>
	/// <param name="assemblyName">The name of the assembly. This output parameter is set to null.</param>
	/// <param name="typeName">The name of the type.</param>
	public void BindToName(
		Type serializedType,
		out string? assemblyName,
		out string typeName)
	{
		assemblyName = null; // Ignore assembly name
		typeName = serializedType.Name;
	}
}