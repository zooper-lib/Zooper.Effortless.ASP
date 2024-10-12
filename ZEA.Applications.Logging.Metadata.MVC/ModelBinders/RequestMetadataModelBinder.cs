using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Accessors;

namespace ZEA.Applications.Logging.Metadata.MVC.ModelBinders;

/// <summary>
/// Model binder for request metadata.
/// </summary>
/// <typeparam name="TMetadata">The type of request metadata.</typeparam>
public class RequestMetadataModelBinder<TMetadata>(IRequestMetadataAccessor<TMetadata> accessor) : IModelBinder
	where TMetadata : IRequestMetadata
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		var metadata = accessor.Metadata;

		bindingContext.Result = metadata != null ? ModelBindingResult.Success(metadata) : ModelBindingResult.Failed();

		return Task.CompletedTask;
	}
}