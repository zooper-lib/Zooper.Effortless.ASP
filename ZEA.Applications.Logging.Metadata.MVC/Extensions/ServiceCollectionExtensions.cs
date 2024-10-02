using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Accessors;
using ZEA.Applications.Logging.Metadata.MVC.Middlewares;
using ZEA.Applications.Logging.Metadata.MVC.Providers;
using ZEA.Applications.Logging.Metadata.MVC.Services;

namespace ZEA.Applications.Logging.Metadata.MVC.Extensions;

/// <summary>
/// Extension methods for registering request metadata services.
/// </summary>
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRequestMetadata<TMetadata, TService>(this IServiceCollection services)
		where TMetadata : class, IRequestMetadata
		where TService : class, IRequestMetadataService<TMetadata>
	{
		services.AddHttpContextAccessor();
		services.AddScoped<IRequestMetadataService<TMetadata>, TService>();
		services.AddScoped<IRequestMetadataAccessor<TMetadata>, HttpContextRequestMetadataAccessor<TMetadata>>();
		services.AddTransient<RequestMetadataMiddleware<TMetadata>>();
		services.AddSingleton<IModelBinderProvider>(
			provider =>
			{
				var accessor = provider.GetRequiredService<IRequestMetadataAccessor<TMetadata>>();
				return new RequestMetadataModelBinderProvider<TMetadata>(accessor);
			}
		);
		return services;
	}
}