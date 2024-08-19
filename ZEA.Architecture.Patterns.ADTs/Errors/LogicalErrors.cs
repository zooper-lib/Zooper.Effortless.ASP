using System.Diagnostics;

namespace ZEA.Architecture.Patterns.ADTs.Errors;

/// <summary>
/// Details of the error that are safe to expose to the user or client.
/// </summary>
/// <param name="Message"></param>
/// <param name="Details"></param>
public record PublicDetails(string Message, string Details);

/// <summary>
/// Details of the error that are only safe to expose to the developers or administrators.
/// </summary>
/// <param name="StackTrace"></param>
public record InternalDetails(StackTrace StackTrace);

/// <summary>
/// Base class representing a logical error within the program flow.
/// </summary>
public abstract record LogicalError(PublicDetails PublicDetails, InternalDetails? InternalDetails = null);

/// <summary>
/// Error representing the situation where a particular action is not allowed in the current context.
/// </summary>
public sealed record ActionNotAllowed(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the failure of authentication due to invalid credentials or other reasons.
/// </summary>
public sealed record AuthenticationFailed(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the violation of a specific business rule or domain constraint.
/// </summary>
public sealed record BusinessRuleViolation(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing a conflict that occurs due to concurrent operations attempting to modify the same resource.
/// </summary>
public sealed record ConcurrencyConflict(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing an issue caused by invalid or missing configuration.
/// </summary>
public sealed record ConfigurationError(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing a conflict that prevents successful completion of an operation.
/// </summary>
public sealed record ConflictError(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Represents an error that occurs when an operation would violate data integrity constraints.
/// </summary>
public sealed record DataIntegrityViolation(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the situation where an entity already exists when a unique entity is expected.
/// </summary>
public sealed record EntityAlreadyExists(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the situation where an entity is not found when it is expected to exist.
/// </summary>
public sealed record EntityNotFound(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing a situation where invalid input is provided by the user or system.
/// </summary>
public sealed record InvalidInput(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing an operation that is not valid in the current state or context.
/// </summary>
public sealed record InvalidOperation(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing an invalid attempt to transition an entity from one state to another.
/// </summary>
public sealed record InvalidStateTransition(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing a situation where an operation times out before completion.
/// </summary>
public sealed record OperationTimedOut(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the denial of an operation due to insufficient permissions.
/// </summary>
public sealed record PermissionDenied(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the failure to meet a required precondition for an operation.
/// </summary>
public sealed record PreconditionFailed(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the situation where a required field is missing in the input or payload.
/// </summary>
public sealed record RequiredFieldMissing(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the unavailability of a service required for the operation.
/// </summary>
public sealed record ServiceUnavailable(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing a conflict in the entity's state that prevents an action from being performed.
/// </summary>
public sealed record StateConflict(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing the denial of access to a resource or operation due to lack of authorization.
/// </summary>
public sealed record UnauthorizedAccess(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing an unknown issue that doesn't fit other specific error categories.
/// </summary>
public sealed record UnknownError(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);

/// <summary>
/// Error representing a situation where the provided data fails to meet validation rules.
/// </summary>
public sealed record ValidationError(PublicDetails PublicDetails, InternalDetails? InternalDetails = null)
	: LogicalError(PublicDetails, InternalDetails);