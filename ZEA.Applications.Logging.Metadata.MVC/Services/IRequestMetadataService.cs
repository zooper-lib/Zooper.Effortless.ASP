using Microsoft.AspNetCore.Http;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

namespace ZEA.Applications.Logging.Metadata.MVC.Services;

/// <summary>
/// Service for retrieving request metadata.
/// </summary>
/// <typeparam name="TMetadata">The type of request metadata.</typeparam>
public interface IRequestMetadataService<TMetadata> where TMetadata : IRequestMetadata
{
	/// <summary>
	/// Asynchronously gets the metadata for the current request.
	/// </summary>
	/// <param name="context">The HTTP context.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The metadata for the current request.</returns>
	Task<TMetadata> GetMetadataAsync(
		HttpContext context,
		CancellationToken cancellationToken);
}