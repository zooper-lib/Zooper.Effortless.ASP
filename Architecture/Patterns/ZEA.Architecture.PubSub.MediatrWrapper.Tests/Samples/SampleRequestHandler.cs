using ZEA.Architecture.PubSub.Abstractions.Interfaces;

namespace ZEA.Architecture.PubSub.MediatrWrapper.Tests.Samples;

public class SampleRequestHandler : IRequestHandler<SampleRequest, string>
{
	public Task<string> HandleAsync(
		SampleRequest request,
		CancellationToken cancellationToken = default)
	{
		return Task.FromResult($"Processed data: {request.Data}");
	}
}