// ReSharper disable UnusedType.Global

namespace ZEA.Techniques.ADTs.Errors;

/// <summary>
/// Error representing a situation where a connection to a service or database failed.
/// </summary>
/// <param name="Message"></param>
/// <param name="Details"></param>
public sealed record ConnectionFailed(string Message, string? Details = null)
	: LogicalError(Message, Details);