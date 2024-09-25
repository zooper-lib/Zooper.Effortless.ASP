using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.Abstractions.Builders;

/// <summary>
/// Provides a fluent API for configuring mediator settings in an application.
/// </summary>
public class MediatorBuilder(IServiceCollection services)
{
	private readonly List<Assembly> _assemblies = [];

	/// <summary>
	/// Adds assemblies to scan for handlers.
	/// If no assemblies are provided, all currently loaded assemblies will be used.
	/// </summary>
	/// <param name="assemblies">The assemblies to scan for handlers.</param>
	/// <returns>The current <see cref="MediatorBuilder"/> instance for chaining further configuration.</returns>
	public MediatorBuilder AddAssemblies(params Assembly[] assemblies)
	{
		if (assemblies is { Length: > 0 })
		{
			_assemblies.AddRange(assemblies);
		}

		return this;
	}

	/// <summary>
	/// Allows implementation-specific service registration without exposing IServiceCollection or assemblies.
	/// </summary>
	/// <param name="configure">An action that receives <see cref="IServiceCollection"/> and assemblies for registration.</param>
	/// <returns>The current <see cref="MediatorBuilder"/> instance for chaining further configuration.</returns>
	public MediatorBuilder ConfigureImplementation(Action<IServiceCollection, IEnumerable<Assembly>> configure)
	{
		if (_assemblies.Count == 0)
		{
			// Use all loaded assemblies if none are provided
			_assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
		}

		configure(services, _assemblies);

		return this;
	}

	/// <summary>
	/// Finalizes the configuration and registers the mediator and handlers.
	/// </summary>
	/// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
	public IServiceCollection Build()
	{
		if (_assemblies.Count == 0)
		{
			// Use all loaded assemblies if none are provided
			_assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
		}

		// Register your handlers based on abstractions
		services.Scan(
			scan => scan
				.FromAssemblies(_assemblies)
				.AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
				.AsImplementedInterfaces()
				.WithTransientLifetime()
		);

		services.Scan(
			scan => scan
				.FromAssemblies(_assemblies)
				.AddClasses(classes => classes.AssignableTo(typeof(INotificationHandler<>)))
				.AsImplementedInterfaces()
				.WithTransientLifetime()
		);

		return services;
	}
}