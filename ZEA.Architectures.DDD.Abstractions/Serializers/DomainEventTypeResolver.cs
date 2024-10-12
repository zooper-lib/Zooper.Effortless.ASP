using System.Reflection;
using ZEA.Architectures.DDD.Abstractions.Attributes;

namespace ZEA.Architectures.DDD.Abstractions.Serializers;

/// <summary>
/// Resolves the appropriate domain event type based on the event name.
/// This utility class is responsible for mapping event names to their corresponding domain event types,
/// using reflection to find types that are annotated with the <see cref="DomainEventNameAttribute"/>.
/// </summary>
public static class DomainEventTypeResolver
{
	/// <summary>
	/// Resolves the <see cref="Type"/> of a domain event based on its name, searching the specified assemblies.
	/// If no assemblies are provided, all loaded assemblies in the current application domain are searched.
	/// </summary>
	/// <param name="eventName">The name of the event to resolve.</param>
	/// <param name="assemblies">Optional parameter list of assemblies to search. If not provided, all loaded assemblies are used.</param>
	/// <returns>The <see cref="Type"/> corresponding to the event name, or <c>null</c> if no matching type is found.</returns>
	public static Type? ResolveEventType(
		string eventName,
		params Assembly[] assemblies)
	{
		// Use provided assemblies or default to all loaded assemblies in the current application domain
		var assembliesToSearch = assemblies.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

		// Find the type with the matching DomainEventNameAttribute
		var domainEventType = assembliesToSearch
			.SelectMany(assembly => assembly.GetTypes())
			.FirstOrDefault(type => type.GetCustomAttribute<DomainEventNameAttribute>()?.EventName == eventName);

		return domainEventType;
	}

	/// <summary>
	/// Resolves the <see cref="Type"/> of a domain event based on its name and version, searching the specified assemblies.
	/// If no assemblies are provided, all loaded assemblies in the current application domain are searched.
	/// </summary>
	/// <param name="eventName">The name of the event to resolve.</param>
	/// <param name="version">The version of the event to resolve.</param>
	/// <param name="assemblies">Optional parameter list of assemblies to search. If not provided, all loaded assemblies are used.</param>
	/// <returns>The <see cref="Type"/> corresponding to the event name, or <c>null</c> if no matching type is found.</returns>
	public static Type? ResolveEventType(
		string eventName,
		int version,
		params Assembly[] assemblies)
	{
		// Use provided assemblies or default to all loaded assemblies in the current application domain
		var assembliesToSearch = assemblies.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

		// Find the type with the matching DomainEventNameAttribute
		var domainEventType = assembliesToSearch
			.SelectMany(assembly => assembly.GetTypes())
			.FirstOrDefault(
				type => type.GetCustomAttribute<DomainEventNameAttribute>()?.EventName == eventName &&
				        type.GetCustomAttribute<DomainEventNameAttribute>()?.Version == version
			);

		return domainEventType;
	}
}