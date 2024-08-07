namespace ZEA.Communication.Messaging.Abstractions;

/// <summary>
/// The metadata for an event.
/// </summary>
public class EventMetadata(Guid id, DateTime createdAt)
{
	public static EventMetadata Create() => new(Guid.NewGuid(), DateTime.UtcNow);

	/// <summary>
	/// Gets the unique identifier for the event.
	/// </summary>
	public Guid Id { get; init; } = id;

	/// <summary>
	/// Gets the date and time when the event was created.
	/// </summary>
	public DateTime CreatedAt { get; init; } = createdAt;
}