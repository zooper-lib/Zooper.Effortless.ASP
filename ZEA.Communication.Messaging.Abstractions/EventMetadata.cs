namespace ZEA.Communication.Messaging.Abstractions;

/// <summary>
/// The metadata for an event.
/// </summary>
public sealed class EventMetadata(Guid id, DateTime createdAt)
{
	public static EventMetadata Create(Guid id, DateTime createdAt) => new(id, createdAt);

	/// <summary>
	/// Gets the unique identifier for the event.
	/// </summary>
	public Guid Id { get; init; } = id;

	/// <summary>
	/// Gets the date and time when the event was created.
	/// </summary>
	public DateTime CreatedAt { get; init; } = createdAt;
}