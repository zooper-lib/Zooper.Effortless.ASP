namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

/// <summary>
/// Defines a handler for a notification of type <typeparamref name="TNotification"/>.
/// </summary>
/// <typeparam name="TNotification">The type of the notification being handled, which implements <see cref="INotification"/>.</typeparam>
public interface INotificationHandler<in TNotification>
{
	/// <summary>
	/// Asynchronously handles the notification.
	/// </summary>
	/// <param name="notification">The notification instance.</param>
	/// <param name="cancellationToken">Optional cancellation token to cancel the notification handling.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}