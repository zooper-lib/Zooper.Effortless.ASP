using System.Reflection;
using ZEA.Architecture.DDD.Attributes;
using ZEA.Architecture.DDD.Interfaces;

namespace ZEA.Architecture.DDD.Extensions;

/// <summary>
/// Provides extension methods for domain events, enabling additional functionality
/// such as retrieving the event name from a domain event instance.
/// </summary>
public static class DomainEventExtensions
{
	/// <summary>
	/// Retrieves the event name from a domain event by reading its <see cref="DomainEventNameAttribute"/>.
	/// </summary>
	/// <param name="domainEvent">The domain event instance to retrieve the name from.</param>
	/// <returns>The event name associated with the domain event.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the event does not have a <see cref="DomainEventNameAttribute"/>.</exception>
	public static string GetEventName(this IDomainEvent domainEvent)
	{
		var type = domainEvent.GetType();
		var attribute = type.GetCustomAttribute<DomainEventNameAttribute>();

		if (attribute is null)
		{
			throw new InvalidOperationException($"Domain event of type {type.FullName} does not have a DomainEventNameAttribute.");
		}

		return attribute.EventName;
	}
}