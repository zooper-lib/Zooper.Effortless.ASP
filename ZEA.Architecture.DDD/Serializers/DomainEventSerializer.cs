using System.Text;
using Newtonsoft.Json;
using ZEA.Architecture.DDD.Interfaces;

namespace ZEA.Architecture.DDD.Serializers;

/// <summary>
/// Provides serialization and deserialization functionality for domain events.
/// This class is responsible for converting domain events to and from JSON format,
/// leveraging custom <see cref="JsonSerializerSettings"/> for configuration.
/// </summary>
public class DomainEventSerializer(JsonSerializerSettings? jsonSettings = null)
{
	private readonly JsonSerializerSettings _jsonSettings = jsonSettings ?? new JsonSerializerSettings();

	/// <summary>
	/// Deserializes a domain event from a JSON-encoded byte array based on the event name.
	/// </summary>
	/// <param name="eventName">The name of the event, used to determine the correct event type.</param>
	/// <param name="eventData">The byte array containing the JSON-encoded event data.</param>
	/// <returns>The deserialized domain event, or <c>null</c> if the event data is empty.</returns>
	/// <exception cref="JsonSerializationException">Thrown if the event type cannot be resolved or if deserialization fails.</exception>
	public IDomainEvent? DeserializeEvent(
		string eventName,
		byte[] eventData)
	{
		var eventType = DomainEventTypeResolver.ResolveEventType(eventName);

		if (eventType == null)
		{
			throw new JsonSerializationException($"No event type found for EventName '{eventName}'.");
		}

		var jsonData = Encoding.UTF8.GetString(eventData);
		return (IDomainEvent?)JsonConvert.DeserializeObject(jsonData, eventType, _jsonSettings);
	}

	/// <summary>
	/// Serializes a domain event into a JSON-encoded byte array.
	/// </summary>
	/// <param name="domainEvent">The domain event to serialize.</param>
	/// <returns>A byte array containing the JSON-encoded event data.</returns>
	public byte[] SerializeEvent(IDomainEvent domainEvent)
	{
		var eventDataJson = JsonConvert.SerializeObject(domainEvent, _jsonSettings);
		return Encoding.UTF8.GetBytes(eventDataJson);
	}
}