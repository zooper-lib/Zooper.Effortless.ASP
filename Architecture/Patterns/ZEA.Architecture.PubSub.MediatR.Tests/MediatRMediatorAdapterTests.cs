using MediatR;
using Moq;
using Xunit;
using ZEA.Architecture.PubSub.Mediat;
using ZEA.Architecture.PubSub.MediatR.Tests.Samples;

namespace ZEA.Architecture.PubSub.MediatR.Tests;

public class MediatRMediatorAdapterTests
{
	[Fact]
	public async Task SendAsync_ForwardsRequestToMediatR()
	{
		// Arrange
		var mediatorMock = new Mock<IMediator>();
		var adapter = new MediatRMediatorAdapter(mediatorMock.Object);
		var request = new SampleRequest
		{
			Data = "Test data"
		};
		var expectedResponse = "Processed data: Test data";

		MediatRRequestAdapter<SampleRequest, string> capturedRequest = null;

		mediatorMock
			.Setup(m => m.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()))
			.Callback<IRequest<string>, CancellationToken>(
				(
					req,
					_) =>
				{
					capturedRequest = req as MediatRRequestAdapter<SampleRequest, string>;
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
		Assert.IsType<MediatRRequestAdapter<SampleRequest, string>>(capturedRequest);
		Assert.Equal(request, capturedRequest.InnerRequest);
	}
}