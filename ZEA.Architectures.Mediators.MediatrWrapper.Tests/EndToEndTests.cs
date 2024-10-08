using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ZEA.Architectures.Mediators.Abstractions.Interfaces;
using ZEA.Architectures.Mediators.MediatrWrapper.Tests.Samples;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Tests;

public class EndToEndTests
{
	[Fact]
	public async Task Mediator_SendAsync_ReturnsExpectedResult()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddApplicationServices();
		var serviceProvider = services.BuildServiceProvider();

		var mediator = serviceProvider.GetRequiredService<IMediator>();
		var request = new SampleRequest
		{
			Data = "End-to-end test data"
		};

		// Act
		var response = await mediator.SendAsync<SampleRequest, string>(request);

		// Assert
		Assert.Equal("Processed data: End-to-end test data", response);
	}

	// [Fact]
	// public async Task Mediator_PublishAsync_CallsNotificationHandler()
	// {
	// 	// Arrange
	// 	var services = new ServiceCollection();
	// 	var notificationHandlerMock = new Mock<IZooperNotificationHandler<SampleNotification>>();
	//
	// 	services.AddMediatR(typeof(Program).Assembly);
	// 	services.AddSingleton<IMediator, MediatrMediatorAdapter>();
	// 	services.AddTransient(typeof(IRequestHandler<,>), typeof(MediatrRequestHandlerAdapter<,>));
	// 	services.AddTransient(typeof(Abstractions.Interfaces.INotificationHandler<MediatrNotificationAdapter>), typeof(MediatRNotificationHandlerAdapter<>));
	//
	// 	// Replace the notification handler with a mock
	// 	services.AddSingleton(notificationHandlerMock.Object);
	//
	// 	var serviceProvider = services.BuildServiceProvider();
	// 	var mediator = serviceProvider.GetRequiredService<IMediator>();
	// 	var notification = new SampleNotification { Message = "End-to-end test notification" };
	//
	// 	// Act
	// 	await mediator.PublishAsync(notification, CancellationToken.None);
	//
	// 	// Assert
	// 	notificationHandlerMock.Verify(h => h.HandleAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
	// }
}