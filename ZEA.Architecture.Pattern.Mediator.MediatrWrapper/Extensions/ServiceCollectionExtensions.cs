using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;
using ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMediatrAdapter(
		this IServiceCollection services,
		params Assembly[] assemblies)
	{
		// Register Mediatr internally
		services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assemblies); });

		// Register the implementation of IMediator
		services.AddSingleton<IMediator, MediatrMediatorAdapter>();

		// Register handler adapters
		services.AddTransient(typeof(IRequestHandler<,>), typeof(MediatrRequestHandlerAdapter<,>));
		services.AddTransient(typeof(INotificationHandler<>), typeof(MediatrNotificationHandlerAdapter<>));

		return services;
	}
}