namespace ZEA.Architecture.Patterns.ADTs.Errors;

/// <summary>
/// Base class representing a logical error within the program flow.
/// </summary>
public abstract record LogicalError;

/// <summary>
/// Error representing the situation where a particular action is not allowed in the current context.
/// </summary>
public sealed record ActionNotAllowed : LogicalError;

/// <summary>
/// Error representing the failure of authentication due to invalid credentials or other reasons.
/// </summary>
public sealed record AuthenticationFailed : LogicalError;

/// <summary>
/// Error representing the violation of a specific business rule or domain constraint.
/// </summary>
public sealed record BusinessRuleViolation : LogicalError;

/// <summary>
/// Error representing a conflict that occurs due to concurrent operations attempting to modify the same resource.
/// </summary>
public sealed record ConcurrencyConflict : LogicalError;

/// <summary>
/// Error representing an issue caused by invalid or missing configuration.
/// </summary>
public sealed record ConfigurationError : LogicalError;

/// <summary>
/// Error representing a conflict that prevents successful completion of an operation.
/// </summary>
public sealed record ConflictError : LogicalError;

/// <summary>
/// Represents an error that occurs when an operation would violate data integrity constraints.
/// </summary>
public sealed record DataIntegrityViolation : LogicalError;

/// <summary>
/// Error representing the situation where an entity already exists when a unique entity is expected.
/// </summary>
public sealed record EntityAlreadyExists : LogicalError;

/// <summary>
/// Error representing the situation where an entity is not found when it is expected to exist.
/// </summary>
public sealed record EntityNotFound : LogicalError;

/// <summary>
/// Error representing a situation where invalid input is provided by the user or system.
/// </summary>
public sealed record InvalidInput : LogicalError;

/// <summary>
/// Error representing an operation that is not valid in the current state or context.
/// </summary>
public sealed record InvalidOperation : LogicalError;

/// <summary>
/// Error representing an invalid attempt to transition an entity from one state to another.
/// </summary>
public sealed record InvalidStateTransition : LogicalError;

/// <summary>
/// Error representing a situation where an operation times out before completion.
/// </summary>
public sealed record OperationTimedOut : LogicalError;

/// <summary>
/// Error representing the denial of an operation due to insufficient permissions.
/// </summary>
public sealed record PermissionDenied : LogicalError;

/// <summary>
/// Error representing the failure to meet a required precondition for an operation.
/// </summary>
public sealed record PreconditionFailed : LogicalError;

/// <summary>
/// Error representing the situation where a required field is missing in the input or payload.
/// </summary>
public sealed record RequiredFieldMissing : LogicalError;

/// <summary>
/// Error representing the unavailability of a service required for the operation.
/// </summary>
public sealed record ServiceUnavailable : LogicalError;

/// <summary>
/// Error representing a conflict in the entity's state that prevents an action from being performed.
/// </summary>
public sealed record StateConflict : LogicalError;

/// <summary>
/// Error representing the denial of access to a resource or operation due to lack of authorization.
/// </summary>
public sealed record UnauthorizedAccess : LogicalError;

/// <summary>
/// Error representing an unknown issue that doesn't fit other specific error categories.
/// </summary>
public sealed record UnknownError : LogicalError;

/// <summary>
/// Error representing a situation where the provided data fails to meet validation rules.
/// </summary>
public sealed record ValidationError : LogicalError;