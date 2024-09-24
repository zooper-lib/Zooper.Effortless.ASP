using ZEA.Architecture.PubSub.Abstractions.Interfaces;

namespace ZEA.Architecture.PubSub.MediatR.Tests.Samples;

public class SampleRequestHandler : IRequestHandler<SampleRequest, string>
{
	public Task<string> HandleAsync(
		SampleRequest request,
		CancellationToken cancellationToken = default)
	{
		return Task.FromResult($"Processed data: {request.Data}");
	}
}