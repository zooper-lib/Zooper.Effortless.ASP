using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace ZEA.Serialization.NewtonsoftJson.Binders;

/// <summary>
/// A flexible serialization binder that maps multiple base type categories to their corresponding types.
/// This is useful when you need to deserialize objects that may implement different interfaces or base types
/// without tightly coupling them to specific assembly names or limiting to a single type hierarchy.
/// </summary>
public sealed class MultiTypeSerializationBinder : ISerializationBinder
{
	private readonly Dictionary<string, Type> _typeCache;

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiTypeSerializationBinder"/> class.
	/// Caches all types in the provided assemblies that implement any of the given base types
	/// and are not interfaces or abstract classes.
	/// </summary>
	/// <param name="baseTypes">An array of base types or interfaces to cache types for (e.g., IEvent, IMetadata).</param>
	/// <param name="assemblies">
	/// Optional: An array of assemblies to scan for types. If no assemblies are provided, it will scan all assemblies
	/// loaded in the current application domain.
	/// </param>
	public MultiTypeSerializationBinder(
		Type[] baseTypes,
		Assembly[]? assemblies = null)
	{
		// If no assemblies are provided, scan all loaded assemblies in the current application domain.
		assemblies = assemblies?.Length == 0 ? AppDomain.CurrentDomain.GetAssemblies() : assemblies;

		// Cache all types that implement the provided base types, ensuring they are neither interfaces nor abstract classes.
		_typeCache = assemblies!
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => baseTypes.Any(baseType => baseType.IsAssignableFrom(type)) && type is { IsInterface: false, IsAbstract: false })
			.ToDictionary(type => type.Name, type => type);
	}

	/// <summary>
	/// Binds a type name (from JSON) to a <see cref="Type"/> object.
	/// This method is called during deserialization to resolve the type from the cached type dictionary.
	/// </summary>
	/// <param name="assemblyName">The name of the assembly. This parameter is ignored in this implementation.</param>
	/// <param name="typeName">The name of the type (as found in the JSON $type property).</param>
	/// <returns>The corresponding <see cref="Type"/> object if found in the cache.</returns>
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
	/// This method is called during serialization to store the type's name in the serialized data.
	/// </summary>
	/// <param name="serializedType">The type to be serialized.</param>
	/// <param name="assemblyName">The name of the assembly. This output parameter is set to null as assembly names are ignored.</param>
	/// <param name="typeName">The name of the type (used for JSON $type).</param>
	public void BindToName(
		Type serializedType,
		out string? assemblyName,
		out string typeName)
	{
		assemblyName = null; // Ignore assembly name
		typeName = serializedType.Name;
	}
}