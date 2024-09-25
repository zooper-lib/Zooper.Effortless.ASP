using System.Diagnostics;

// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedType.Global

namespace ZEA.Architecture.Pattern.ADTs.Errors;

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

// TODO: Move the Implementations to their own files
/// <summary>
/// Error representing the situation where a particular action is not allowed in the current context.
/// </summary>
public sealed record ActionNotAllowed(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ActionNotAllowed"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ActionNotAllowed(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ActionNotAllowed"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ActionNotAllowed(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the failure of authentication due to invalid credentials or other reasons.
/// </summary>
public sealed record AuthenticationFailed(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticationFailed"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public AuthenticationFailed(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthenticationFailed"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public AuthenticationFailed(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the violation of a specific business rule or domain constraint.
/// </summary>
public sealed record BusinessRuleViolation(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BusinessRuleViolation"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public BusinessRuleViolation(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BusinessRuleViolation"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public BusinessRuleViolation(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing a conflict that occurs due to concurrent operations attempting to modify the same resource.
/// </summary>
public sealed record ConcurrencyConflict(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConcurrencyConflict"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ConcurrencyConflict(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConcurrencyConflict"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ConcurrencyConflict(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing an issue caused by invalid or missing configuration.
/// </summary>
public sealed record ConfigurationError(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigurationError"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ConfigurationError(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigurationError"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ConfigurationError(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing a conflict that prevents successful completion of an operation.
/// </summary>
public sealed record ConflictError(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ConflictError"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ConflictError(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConflictError"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ConflictError(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Represents an error that occurs when an operation would violate data integrity constraints.
/// </summary>
public sealed record DataIntegrityViolation(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DataIntegrityViolation"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public DataIntegrityViolation(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DataIntegrityViolation"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public DataIntegrityViolation(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the situation where an entity already exists when a unique entity is expected.
/// </summary>
public sealed record EntityAlreadyExists(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityAlreadyExists"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public EntityAlreadyExists(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EntityAlreadyExists"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public EntityAlreadyExists(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the situation where an entity is not found when it is expected to exist.
/// </summary>
public sealed record EntityNotFound(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityNotFound"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public EntityNotFound(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EntityNotFound"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public EntityNotFound(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing a situation where invalid input is provided by the user or system.
/// </summary>
public sealed record InvalidInput(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidInput"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public InvalidInput(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidInput"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public InvalidInput(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing an operation that is not valid in the current state or context.
/// </summary>
public sealed record InvalidOperation(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidOperation"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public InvalidOperation(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidOperation"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public InvalidOperation(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing an invalid attempt to transition an entity from one state to another.
/// </summary>
public sealed record InvalidStateTransition(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidStateTransition"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public InvalidStateTransition(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InvalidStateTransition"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public InvalidStateTransition(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing a situation where an operation times out before completion.
/// </summary>
public sealed record OperationTimedOut(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OperationTimedOut"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public OperationTimedOut(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OperationTimedOut"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public OperationTimedOut(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the denial of an operation due to insufficient permissions.
/// </summary>
public sealed record PermissionDenied(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PermissionDenied"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public PermissionDenied(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PermissionDenied"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public PermissionDenied(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the failure to meet a required precondition for an operation.
/// </summary>
public sealed record PreconditionFailed(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PreconditionFailed"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public PreconditionFailed(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PreconditionFailed"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public PreconditionFailed(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the situation where a required field is missing in the input or payload.
/// </summary>
public sealed record RequiredFieldMissing(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredFieldMissing"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public RequiredFieldMissing(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RequiredFieldMissing"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public RequiredFieldMissing(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the unavailability of a service required for the operation.
/// </summary>
public sealed record ServiceUnavailable(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceUnavailable"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ServiceUnavailable(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceUnavailable"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ServiceUnavailable(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing a conflict in the entity's state that prevents an action from being performed.
/// </summary>
public sealed record StateConflict(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="StateConflict"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public StateConflict(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StateConflict"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public StateConflict(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing the denial of access to a resource or operation due to lack of authorization.
/// </summary>
public sealed record UnauthorizedAccess(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UnauthorizedAccess"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public UnauthorizedAccess(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnauthorizedAccess"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public UnauthorizedAccess(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing an unknown issue that doesn't fit other specific error categories.
/// </summary>
public sealed record UnknownError(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UnknownError"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public UnknownError(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnknownError"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public UnknownError(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}

/// <summary>
/// Error representing a situation where the provided data fails to meet validation rules.
/// </summary>
public sealed record ValidationError(string Message, string? Details = null)
	: LogicalError(Message, Details)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValidationError"/> record with PublicDetails and InternalDetails.
	/// </summary>
	/// <param name="publicDetails">The public details of the error.</param>
	/// <param name="internalDetails">The internal details of the error.</param>
	[Obsolete("We should not expose InternalDetails (the Exception) to the client.")]
	public ValidationError(
		PublicDetails publicDetails,
		InternalDetails? internalDetails = null)
		: this(publicDetails.Message, publicDetails.Details)
	{
		InternalDetails = internalDetails;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ValidationError"/> record with message, details, and an optional exception.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="details">Additional details about the error.</param>
	/// <param name="ex">The exception associated with the error.</param>
	[Obsolete(
		"This constructor is deprecated and will be removed in a future version. Use the constructor without the Exception parameter instead."
	)]
	public ValidationError(
		string message,
		string? details = null,
		Exception? ex = null)
		: this(message, details)
	{
		if (ex != null)
		{
			InternalDetails = new(new(ex, true));
		}
	}
}