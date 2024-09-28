using System.Diagnostics;

// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedType.Global

namespace ZEA.Techniques.ADTs.Errors;

/// <summary>
/// Details of the error that are safe to expose to the user or client.
/// </summary>
/// <param name="Message"></param>
/// <param name="Details"></param>
// ReSharper disable once ClassNeverInstantiated.Global
[Obsolete("Since we no longer will have InternalDetails, there is no reason to wrap the message and details in a record.")]
public record PublicDetails(string Message, string? Details = null);

/// <summary>
/// Details of the error that are only safe to expose to the developers or administrators.
/// </summary>
/// <param name="StackTrace"></param>
// ReSharper disable once ClassNeverInstantiated.Global
[Obsolete(
	"Will be removed in the next major version. " +
	"The reason for this is that it is not a good practice to expose internal details of an error to the client or any part of the application. " +
	"Better to log it and expose a generic error message to the client."
)]
public record InternalDetails(StackTrace StackTrace);

/// <summary>
/// Base class representing a logical error within the program flow.
/// </summary>
public abstract record LogicalError(string Message, string? Details = null)
{
	public InternalDetails? InternalDetails { get; protected init; }

	protected LogicalError(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null) : this(publicDetails.Message, publicDetails.Details, null)
	{
		InternalDetails = internalDetails;
	}

	// ReSharper disable once MemberCanBePrivate.Global
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	protected LogicalError(
		string message,
		string? details = null,
		Exception? ex = null) : this(message, details)
	{
		InternalDetails = ex != null ? new InternalDetails(new(ex)) : null;
	}
}