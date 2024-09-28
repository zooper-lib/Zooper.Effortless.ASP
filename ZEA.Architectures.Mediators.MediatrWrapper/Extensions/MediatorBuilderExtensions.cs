using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architectures.Mediators.Abstractions.Builders;
using ZEA.Architectures.Mediators.MediatrWrapper.Adapters;
using ZEA.Validations.FluentValidation;
using IMediator = ZEA.Architectures.Mediators.Abstractions.Interfaces.IMediator;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Extensions;

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
				services.AddTransient(typeof(Abstractions.Interfaces.IRequestHandler<,>), typeof(MediatrRequestHandlerAdapter<,>));
				services.AddTransient(typeof(Abstractions.Interfaces.INotificationHandler<>), typeof(MediatrNotificationHandlerAdapter<>));
			}
		);

		return builder;
	}

	// TODO: Check this method if it is working. Also for the Behavior.
	public static MediatorBuilder UseFluentValidationBehavior(this MediatorBuilder builder)
	{
		builder.ConfigureImplementation(
			(
				services,
				assemblies) =>
			{
				// Register all classes that inherit from AbstractValidator
				services.Scan(
					scan => scan.FromAssemblies(assemblies)
						.AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
						.AsImplementedInterfaces()
						.WithTransientLifetime()
				);

				// Register the ValidationBehavior for MediatR
				services.AddTransient(
					typeof(IPipelineBehavior<,>),
					typeof(ValidationBehavior<,>)
				);
			}
		);

		return builder;
	}
}