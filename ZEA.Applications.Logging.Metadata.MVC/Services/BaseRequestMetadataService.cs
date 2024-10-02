using Microsoft.AspNetCore.Http;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

namespace ZEA.Applications.Logging.Metadata.MVC.Services;

/// <summary>
/// Base class for implementing request metadata services.
/// Applications should extend this class to provide custom metadata extraction logic.
/// </summary>
/// <typeparam name="TMetadata">The type of metadata.</typeparam>
public abstract class BaseRequestMetadataService<TMetadata> : IRequestMetadataService<TMetadata> where TMetadata : IRequestMetadata
{
	/// <summary>
	/// Extracts metadata from the current HTTP context.
	/// </summary>
	/// <param name="context">The HTTP context.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public abstract Task<TMetadata> GetMetadataAsync(
		HttpContext context,
		CancellationToken cancellationToken);

	protected Guid GetSessionId(HttpContext context)
	{
		// Extract session ID logic
		return Guid.Parse(context.Session.Id);
	}

	protected DateTime GetTimestamp()
	{
		// Get the current timestamp
		return DateTime.UtcNow;
	}

	protected string GetIpAddress(HttpContext context)
	{
		return context.Connection.RemoteIpAddress?.MapToIPv4().ToString()
		       ?? context.Connection.LocalIpAddress?.MapToIPv4().ToString()
		       ?? "0.0.0.0";
	}

	// Additional common methods...
}