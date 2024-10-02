using Microsoft.AspNetCore.Builder;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Middlewares;

namespace ZEA.Applications.Logging.Metadata.MVC.Extensions;

/// <summary>
/// Extension methods for adding middleware to the application pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseRequestMetadata<TMetadata>(this IApplicationBuilder app) where TMetadata : IRequestMetadata
	{
		return app.UseMiddleware<RequestMetadataMiddleware<TMetadata>>();
	}
}