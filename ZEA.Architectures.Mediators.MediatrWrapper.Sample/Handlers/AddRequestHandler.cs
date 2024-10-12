using ZEA.Architectures.Mediators.Abstractions.Interfaces;
using ZEA.Architectures.Mediators.MediatrWrapper.Sample.Requests;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Sample.Handlers;

internal sealed class AddRequestHandler : IRequestHandler<AddRequest, double>
{
	public Task<double> HandleAsync(
		AddRequest request,
		CancellationToken cancellationToken = default)
	{
		return Task.FromResult(request.SummandOne + request.SummandTwo);
	}
}