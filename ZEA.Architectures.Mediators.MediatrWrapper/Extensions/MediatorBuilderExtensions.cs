using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architectures.Mediators.Abstractions.Builders;
using ZEA.Architectures.Mediators.MediatrWrapper.Adapters;
using ZEA.Validations.FluentValidation;

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
        public static MediatorBuilder UseMediatR(this MediatorBuilder builder)
        {
            builder.UseImplementation((services, assemblies) =>
            {
                // Convert assemblies to an array
                var assembliesArray = assemblies.ToArray();

                // Register MediatR with the collected assemblies
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblies(assembliesArray);
                });

                // Register the adapter for IMediator
                services.AddSingleton<Abstractions.Interfaces.IMediator, MediatrMediatorAdapter>();

                // Register handler adapters if needed
                services.AddTransient(typeof(Abstractions.Interfaces.IRequestHandler<,>), typeof(MediatrRequestHandlerAdapter<,>));
                services.AddTransient(typeof(Abstractions.Interfaces.INotificationHandler<>), typeof(MediatrNotificationHandlerAdapter<>));
            });

            return builder;
        }

        /// <summary>
        /// Configures the mediator to use FluentValidation behavior.
        /// </summary>
        public static MediatorBuilder UseFluentValidationBehavior(this MediatorBuilder builder, params Type[] validatorTypes)
        {
            builder.UseImplementation((services, assemblies) =>
            {
                // Register the validators
                foreach (var validatorType in validatorTypes)
                {
                    var interfaces = validatorType.GetInterfaces().Where(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(FluentValidation.IValidator<>));

                    foreach (var @interface in interfaces)
                    {
                        services.AddTransient(@interface, validatorType);
                    }
                }

                // Register the ValidationBehavior for MediatR
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            return builder;
        }
    }