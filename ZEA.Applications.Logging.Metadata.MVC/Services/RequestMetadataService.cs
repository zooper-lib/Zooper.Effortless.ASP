using Microsoft.AspNetCore.Http;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

// ReSharper disable ClassNeverInstantiated.Global

namespace ZEA.Applications.Logging.Metadata.MVC.Services;

/// <summary>
/// An example implementation of a request metadata service.
/// </summary>
public class RequestMetadataService : IRequestMetadataService<RequestMetadata>
{
	public Task<RequestMetadata> GetMetadataAsync(
		HttpContext context,
		CancellationToken cancellationToken)
	{
		var sessionId = GetSessionId(context);
		var timestamp = GetTimestamp();

		return Task.FromResult(
			new RequestMetadata
			{
				SessionId = sessionId,
				Timestamp = timestamp,
			}
		);
	}

	private static Guid GetSessionId(HttpContext context)
	{
		// Extract session ID logic
		return Guid.Parse(context.Session.Id);
	}

	private static DateTime GetTimestamp()
	{
		// Get the current timestamp
		return DateTime.UtcNow;
	}
}