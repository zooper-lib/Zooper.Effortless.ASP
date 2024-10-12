using Microsoft.AspNetCore.Http;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;

namespace ZEA.Applications.Logging.Metadata.MVC.Accessors;

/// <summary>
/// Accessor that stores request metadata in the HTTP context.
/// </summary>
/// <typeparam name="TMetadata">The type of request metadata.</typeparam>
public class HttpContextRequestMetadataAccessor<TMetadata>(IHttpContextAccessor httpContextAccessor) : IRequestMetadataAccessor<TMetadata>
	where TMetadata : class, IRequestMetadata
{
	private const string MetadataKey = "RequestMetadata";

	public TMetadata? Metadata
	{
		get
		{
			var context = httpContextAccessor.HttpContext;
			return context?.Items[MetadataKey] as TMetadata;
		}
		set
		{
			var context = httpContextAccessor.HttpContext;

			if (context != null)
			{
				context.Items[MetadataKey] = value;
			}
		}
	}
}