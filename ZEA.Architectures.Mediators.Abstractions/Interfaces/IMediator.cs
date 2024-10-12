namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

/// <summary>
/// Interface for sending requests and publishing notifications in a mediator pattern.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Asynchronously sends a request to a single handler and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request object that implements <see cref="IRequest{TResponse}"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, with the handler's response.</returns>
    Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sends a request to a single handler and returns the response, specifying both request and response types.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request that implements <see cref="IRequest{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request object.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, with the handler's response.</returns>
    Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;

    /// <summary>
    /// Asynchronously publishes a notification to multiple handlers.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification that implements <see cref="INotification"/>.</typeparam>
    /// <param name="notification">The notification object.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the notification.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}