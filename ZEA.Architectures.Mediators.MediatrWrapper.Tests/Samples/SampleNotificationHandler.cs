using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Tests.Samples;

public class SampleNotificationHandler : INotificationHandler<SampleNotification>
{
	public Task HandleAsync(
		SampleNotification notification,
		CancellationToken cancellationToken = default)
	{
		// Handle the notification
		Console.WriteLine($"Notification received: {notification.Message}");
		return Task.CompletedTask;
	}
}