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

	/// <summary>
	/// The metadata of the requester who made the request
	/// </summary>
	public required IRequesterMetadata RequesterMetadata { get; init; }
}

/// <summary>
/// The metadata of the requester who made the request
/// </summary>
public interface IRequesterMetadata;

/// <summary>
/// Metadata of a request made by a user (client application)
/// </summary>
public record UserRequestMetadata : IRequesterMetadata
{
	/// <summary>
	/// The ID of the user who made the request
	/// </summary>
	public required string RequesterId { get; init; }

	/// <summary>
	/// The roles of the user who made the request
	/// </summary>
	public required IEnumerable<string> Roles { get; init; }

	/// <summary>
	/// The IP address of the user who made the request
	/// </summary>
	public required string IpAddress { get; init; }

	// /// <summary>
	// /// The location consisting of the city, region, and country of the user
	// /// </summary>
	// public required string Location { get; init; }
}

/// <summary>
/// Metadata of a request made by a service. Useful for tracking requests made by microservices.
/// </summary>
public record ServiceRequestMetadata : IRequesterMetadata
{
	/// <summary>
	/// The name of the calling service
	/// </summary>
	public required string ServiceName { get; init; }

	/// <summary>
	/// The instance ID of the calling service. This could be the Kubernetes Pod name or ID
	/// </summary>
	public required string InstanceId { get; init; }

	/// <summary>
	/// The instance IP of the calling service. This could be the Kubernetes Pod IP
	/// </summary>
	public required string IpAddress { get; init; }
}