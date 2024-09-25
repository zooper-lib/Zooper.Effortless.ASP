using MediatR;
using Moq;
using Xunit;
using ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;
using ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests.Samples;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests;

public class MediatrMediatorAdapterTests
{
	[Fact]
	public async Task SendAsync_ForwardsRequestToMediatR()
	{
		// Arrange
		var mediatorMock = new Mock<IMediator>();
		var adapter = new MediatrMediatorAdapter(mediatorMock.Object);
		var request = new SampleRequest
		{
			Data = "Test data"
		};
		var expectedResponse = "Processed data: Test data";

		MediatrRequestAdapter<SampleRequest, string> capturedRequest = null;

		mediatorMock
			.Setup(m => m.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()))
			.Callback<IRequest<string>, CancellationToken>(
				(
					req,
					_) =>
				{
					capturedRequest = req as MediatrRequestAdapter<SampleRequest, string>;
				}
			)
			.ReturnsAsync(expectedResponse);

		// Act
		var response = await adapter.SendAsync<SampleRequest, string>(request, CancellationToken.None);

		// Assert
		Assert.Equal(expectedResponse, response);
		mediatorMock.Verify(
			m => m.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()),
			Times.Once
		);

		Assert.NotNull(capturedRequest);
		Assert.IsType<MediatrRequestAdapter<SampleRequest, string>>(capturedRequest);
		Assert.Equal(request, capturedRequest.InnerRequest);
	}
}