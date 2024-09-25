using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests.Samples;

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