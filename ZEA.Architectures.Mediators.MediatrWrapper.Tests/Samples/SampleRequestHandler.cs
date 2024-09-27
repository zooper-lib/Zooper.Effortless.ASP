using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Tests.Samples;

public class SampleRequestHandler : IRequestHandler<SampleRequest, string>
{
	public Task<string> HandleAsync(
		SampleRequest request,
		CancellationToken cancellationToken = default)
	{
		return Task.FromResult($"Processed data: {request.Data}");
	}
}