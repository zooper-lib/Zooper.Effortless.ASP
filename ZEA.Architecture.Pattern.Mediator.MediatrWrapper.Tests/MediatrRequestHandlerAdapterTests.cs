using Moq;
using Xunit;
using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;
using ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;
using ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests.Samples;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests;

public class MediatrRequestHandlerAdapterTests
{
	[Fact]
	public async Task Handle_CallsCustomHandler()
	{
		// Arrange
		var customHandlerMock = new Mock<IRequestHandler<SampleRequest, string>>();
		var adapter = new MediatrRequestHandlerAdapter<SampleRequest, string>(customHandlerMock.Object);
		var innerRequest = new SampleRequest
		{
			Data = "Test data"
		};
		var adapterRequest = new MediatrRequestAdapter<SampleRequest, string>(innerRequest);
		var expectedResponse = "Processed data: Test data";

		customHandlerMock
			.Setup(h => h.HandleAsync(innerRequest, It.IsAny<CancellationToken>()))
			.ReturnsAsync(expectedResponse);

		// Act
		var response = await adapter.Handle(adapterRequest, CancellationToken.None);

		// Assert
		Assert.Equal(expectedResponse, response);
		customHandlerMock.Verify(h => h.HandleAsync(innerRequest, It.IsAny<CancellationToken>()), Times.Once);
	}
}