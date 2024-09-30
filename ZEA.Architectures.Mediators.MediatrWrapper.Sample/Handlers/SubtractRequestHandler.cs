using ZEA.Architectures.Mediators.Abstractions.Interfaces;
using ZEA.Architectures.Mediators.MediatrWrapper.Sample.Requests;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Sample.Handlers;

internal sealed class SubtractRequestHandler : IRequestHandler<SubtractRequest, double>
{
	public Task<double> HandleAsync(
		SubtractRequest request,
		CancellationToken cancellationToken = default)
	{
		return Task.FromResult(request.Minuend - request.Subtrahend);
	}
}