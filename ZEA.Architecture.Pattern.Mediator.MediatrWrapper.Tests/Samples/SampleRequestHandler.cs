using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests.Samples;

public class SampleRequestHandler : IRequestHandler<SampleRequest, string>
{
	public Task<string> HandleAsync(
		SampleRequest request,
		CancellationToken cancellationToken = default)
	{
		return Task.FromResult($"Processed data: {request.Data}");
	}
}