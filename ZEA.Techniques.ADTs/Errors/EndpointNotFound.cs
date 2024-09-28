// ReSharper disable UnusedType.Global

namespace ZEA.Techniques.ADTs.Errors;

/// <summary>
/// Error representing a situation where an endpoint is not found.
/// </summary>
/// <param name="Message"></param>
/// <param name="Details"></param>
public sealed record EndpointNotFound(string Message, string? Details = null)
	: LogicalError(Message, Details);