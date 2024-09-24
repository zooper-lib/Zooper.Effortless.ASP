using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.PubSub.MediatrWrapper.Tests.Samples;
using IMediator = ZEA.Architecture.PubSub.Abstractions.Interfaces.IMediator;

namespace ZEA.Architecture.PubSub.MediatrWrapper.Tests;

public static class ServiceRegistration
{
	public static void AddApplicationServices(this IServiceCollection services)
	{
		// Register MediatR
		services.AddMediatR(
			config => { config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()); }
		);

		// Register the mediator adapter
		services.AddSingleton<IMediator, MediatrMediatorAdapter>();

		// Register your custom handlers
		services.AddTransient<Abstractions.Interfaces.IRequestHandler<SampleRequest, string>, SampleRequestHandler>();

		// Register the handler adapter for the specific types
		services
			.AddTransient<global::MediatR.IRequestHandler<MediatrRequestAdapter<SampleRequest, string>, string>,
				MediatrRequestHandlerAdapter<SampleRequest, string>>();
	}
}