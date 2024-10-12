using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.Abstractions.Builders;

/// <summary>
/// Provides a fluent API for configuring mediator settings in an application.
/// </summary>
public class MediatorBuilder(IServiceCollection services)
{
	// Lists to store the handler types
	private readonly List<Type> _requestHandlerTypes = [];
	private readonly List<Type> _notificationHandlerTypes = [];

	// HashSet to store the assemblies containing the handlers
	private readonly HashSet<Assembly> _handlerAssemblies = [];

	// Implementation configuration action
	private Action<IServiceCollection, IEnumerable<Assembly>>? _implementationConfiguration;

	// Flag to check if an implementation has been configured
	private bool _implementationConfigured;

	/// <summary>
	/// Adds request handlers from the specified assemblies.
	/// If no assemblies are provided, all loaded assemblies are scanned.
	/// </summary>
	public MediatorBuilder AddRequestHandlersFromAssemblies(params Assembly[] assemblies)
	{
		var assembliesToScan = assemblies.Length > 0
			? assemblies
			: AppDomain.CurrentDomain.GetAssemblies();

		foreach (var assembly in assembliesToScan)
		{
			var handlerTypes = assembly.GetTypes()
				.Where(type => type is { IsAbstract: false, IsInterface: false })
				.Where(
					type => type.GetInterfaces().Any(
						i =>
							i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
					)
				)
				.ToList();

			_requestHandlerTypes.AddRange(handlerTypes);
			_handlerAssemblies.Add(assembly);
		}

		return this;
	}

	/// <summary>
	/// Adds specific request handler types.
	/// </summary>
	public MediatorBuilder AddRequestHandlers(params Type[] handlerTypes)
	{
		_requestHandlerTypes.AddRange(handlerTypes);

		foreach (var handlerType in handlerTypes)
		{
			_handlerAssemblies.Add(handlerType.Assembly);
		}

		return this;
	}

	/// <summary>
	/// Adds notification handlers from the specified assemblies.
	/// If no assemblies are provided, all loaded assemblies are scanned.
	/// </summary>
	public MediatorBuilder AddNotificationHandlersFromAssemblies(params Assembly[] assemblies)
	{
		var assembliesToScan = assemblies.Length > 0
			? assemblies
			: AppDomain.CurrentDomain.GetAssemblies();

		foreach (var assembly in assembliesToScan)
		{
			var handlerTypes = assembly.GetTypes()
				.Where(type => type is { IsAbstract: false, IsInterface: false })
				.Where(
					type => type.GetInterfaces().Any(
						i =>
							i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
					)
				)
				.ToList();

			_notificationHandlerTypes.AddRange(handlerTypes);
			_handlerAssemblies.Add(assembly);
		}

		return this;
	}

	/// <summary>
	/// Adds specific notification handler types.
	/// </summary>
	public MediatorBuilder AddNotificationHandlers(params Type[] handlerTypes)
	{
		_notificationHandlerTypes.AddRange(handlerTypes);

		foreach (var handlerType in handlerTypes)
		{
			_handlerAssemblies.Add(handlerType.Assembly);
		}

		return this;
	}

	/// <summary>
	/// Configures the mediator to use the specified implementation.
	/// </summary>
	public MediatorBuilder UseImplementation(Action<IServiceCollection, IEnumerable<Assembly>> configureImplementation)
	{
		_implementationConfiguration = configureImplementation;
		_implementationConfigured = true;
		return this;
	}

	/// <summary>
	/// Finalizes the configuration and registers the mediator and handlers.
	/// </summary>
	public IServiceCollection Build()
	{
		if (!_implementationConfigured)
		{
			throw new InvalidOperationException(
				"No mediator implementation has been configured. Please configure an implementation using an implementation-specific method like 'UseMediatR'."
			);
		}

		// Execute the implementation configuration, passing the collected assemblies
		_implementationConfiguration?.Invoke(services, _handlerAssemblies);

		// Register request handlers
		foreach (var handlerType in _requestHandlerTypes)
		{
			var interfaces = handlerType.GetInterfaces().Where(
				i =>
					i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
			);

			foreach (var @interface in interfaces)
			{
				services.AddTransient(@interface, handlerType);
			}
		}

		// Register notification handlers
		foreach (var handlerType in _notificationHandlerTypes)
		{
			var interfaces = handlerType.GetInterfaces().Where(
				i =>
					i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
			);

			foreach (var @interface in interfaces)
			{
				services.AddTransient(@interface, handlerType);
			}
		}

		return services;
	}
}