using ZEA.Techniques.ADTs;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

public interface IPreProcessor;

public interface IPreProcessor<in TRequest, TResponse>
{
	/// <summary>
	/// Processes the request. If pre-processing passes, returns null. If it fails, returns a TResponse representing the error.
	/// </summary>
	Task<TResponse?> ProcessAsync(
		TRequest request,
		CancellationToken cancellationToken);
}