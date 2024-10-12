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
	/// <summary>
	/// Registers the request metadata services.
	/// </summary>
	/// <param name="services"></param>
	/// <typeparam name="TMetadata"></typeparam>
	/// <typeparam name="TService"></typeparam>
	/// <returns></returns>
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

	/// <summary>
	/// Registers the request metadata services by using the specified request metadata service instance.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="requestMetadataService"></param>
	/// <typeparam name="TMetadata"></typeparam>
	/// <returns></returns>
	public static IServiceCollection AddRequestMetadata<TMetadata>(
		this IServiceCollection services,
		IRequestMetadataService<TMetadata> requestMetadataService)
		where TMetadata : class, IRequestMetadata
	{
		services.AddHttpContextAccessor();
		services.AddScoped<IRequestMetadataService<TMetadata>>(_ => requestMetadataService);
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