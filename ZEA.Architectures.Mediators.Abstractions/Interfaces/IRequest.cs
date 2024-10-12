namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

/// <summary>
/// Marker interface for a request, which represents a query or command that can be sent to a single handler.
/// </summary>
public interface IRequest;

/// <summary>
/// Defines a request with a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response expected from the request.</typeparam>
public interface IRequest<out TResponse> : IRequest;