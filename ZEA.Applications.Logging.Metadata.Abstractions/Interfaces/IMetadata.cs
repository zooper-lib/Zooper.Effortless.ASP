namespace ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

/// <summary>
/// An interface that represents some sort of metadata
/// </summary>
public interface IMetadata;

/// <summary>
/// An interface that represents the metadata of a request
/// </summary>
public interface IRequestMetadata : IMetadata;

/// <summary>
/// An example of the metadata of a request
/// </summary>
public record RequestMetadata : IRequestMetadata
{
	/// <summary>
	/// The ID of the session in which the request was made. Useful for tracking user activity across different events
	/// </summary>
	public required Guid SessionId { get; init; }

	/// <summary>
	/// The date and time at which the event was created
	/// </summary>
	public required DateTime Timestamp { get; init; }
}
