using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architectures.Mediators.Abstractions.Interfaces;
using ZEA.Architectures.Mediators.MediatrWrapper.Adapters;
using ZEA.Architectures.Mediators.MediatrWrapper.Tests.Samples;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Tests;

public static class ServiceRegistration
{
	public static void AddApplicationServices(this IServiceCollection services)
	{
		// Register Mediatr
		services.AddMediatR(
			config => { config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); }
		);

		// Register the mediator adapter
		services.AddSingleton<IMediator, MediatrMediatorAdapter>();

		// Register custom request handlers
		services.AddTransient<IRequestHandler<SampleRequest, string>, SampleRequestHandler>();

		// Register custom notification handlers
		services.AddTransient<INotificationHandler<SampleNotification>, SampleNotificationHandler>();

		// Register the handler adapter for the specific types
		services
			.AddTransient<global::MediatR.IRequestHandler<MediatrRequestAdapter<SampleRequest, string>, string>,
				MediatrRequestHandlerAdapter<SampleRequest, string>>();
	}
}