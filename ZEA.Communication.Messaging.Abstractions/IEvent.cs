namespace ZEA.Communication.Messaging.Abstractions;

/// <summary>
/// Represents an integration event that is used for communication between microservices
/// in a distributed, event-driven architecture.
/// </summary>
public interface IEvent
{
	/// <summary>
	/// The metadata of the event.
	/// </summary>
	EventMetadata EventMetadata { get; }
}