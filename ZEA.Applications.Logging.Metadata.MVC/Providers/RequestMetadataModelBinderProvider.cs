using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Accessors;
using ZEA.Applications.Logging.Metadata.MVC.ModelBinders;

namespace ZEA.Applications.Logging.Metadata.MVC.Providers;

/// <summary>
/// Model binder provider for request metadata.
/// </summary>
/// <typeparam name="TMetadata">The type of request metadata.</typeparam>
public class RequestMetadataModelBinderProvider<TMetadata>(IRequestMetadataAccessor<TMetadata> accessor) : IModelBinderProvider
	where TMetadata : IRequestMetadata
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		return context.Metadata.ModelType == typeof(TMetadata) ? new RequestMetadataModelBinder<TMetadata>(accessor) : null;
	}
}