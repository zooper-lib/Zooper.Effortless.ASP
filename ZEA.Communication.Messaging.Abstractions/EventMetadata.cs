namespace ZEA.Communication.Messaging.Abstractions;

/// <summary>
/// The metadata for an event.
/// </summary>
public sealed class EventMetadata
{
	/// <summary>
	/// Gets the unique identifier for the event.
	/// </summary>
	public Guid Id { get; init; } = Guid.NewGuid();

	/// <summary>
	/// Gets the date and time when the event was created.
	/// </summary>
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}