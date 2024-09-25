using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.Pattern.Mediator.Abstractions.Builders;
using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;
using ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Extensions;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
public static class MediatorBuilderExtensions
{
	/// <summary>
	/// Configures the mediator to use MediatR as the underlying implementation.
	/// </summary>
	/// <param name="builder">The mediator builder.</param>
	/// <returns>The updated <see cref="MediatorBuilder"/> instance.</returns>
	public static MediatorBuilder UseMediatR(this MediatorBuilder builder)
	{
		builder.ConfigureImplementation(
			(
				services,
				assemblies) =>
			{
				// Register Mediatr
				services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assemblies.ToArray()); });

				// Register the implementation of IMediator
				services.AddSingleton<IMediator, MediatrMediatorAdapter>();

				// Register handler adapters
				services.AddTransient(typeof(IRequestHandler<,>), typeof(MediatrRequestHandlerAdapter<,>));
				services.AddTransient(typeof(INotificationHandler<>), typeof(MediatrNotificationHandlerAdapter<>));
			}
		);

		return builder;
	}
}