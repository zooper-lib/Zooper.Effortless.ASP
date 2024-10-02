using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Accessors;
using ZEA.Applications.Logging.Metadata.MVC.Services;

namespace ZEA.Applications.Logging.Metadata.MVC.Middlewares;

/// <summary>
/// Middleware that captures and stores request metadata.
/// </summary>
/// <typeparam name="TMetadata">The type of request metadata.</typeparam>
public class RequestMetadataMiddleware<TMetadata>(
	IRequestMetadataService<TMetadata> requestMetadataService,
	IRequestMetadataAccessor<TMetadata> accessor,
	ILogger<RequestMetadataMiddleware<TMetadata>> logger)
	: IMiddleware
	where TMetadata : IRequestMetadata
{
	public async Task InvokeAsync(
		HttpContext context,
		RequestDelegate next)
	{
		try
		{
			var metadata = await requestMetadataService.GetMetadataAsync(context, context.RequestAborted);
			accessor.Metadata = metadata;

			await next(context);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to set request metadata.");
			throw;
		}
	}
}