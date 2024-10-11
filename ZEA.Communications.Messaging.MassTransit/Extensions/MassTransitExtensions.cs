using System.Reflection;
using System.Text;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Communications.Messaging.MassTransit.Attributes;
using ZEA.Communications.Messaging.MassTransit.Builders;

namespace ZEA.Communications.Messaging.MassTransit.Extensions;

[Obsolete("This class and all containing methods are obsolete. Use the Generators instead for all the functionality.")]
public static class MassTransitExtensions
{
	[Obsolete("Use 'ServiceCollection.AddMassTransit' instead.")]
	public static IServiceCollection AddMassTransitServices(
		this IServiceCollection services,
		Action<MassTransitBuilder> configure)
	{
		var builder = new MassTransitBuilder(services);
		configure(builder);
		return services;
	}

	/// <summary>
	/// Scans the specified assemblies for consumer types and registers them with MassTransit.
	/// </summary>
	/// <param name="configurator">The MassTransit registration configurator.</param>
	/// <param name="assembliesToScan">Assemblies to scan for consumers. If none specified, scans all loaded assemblies.</param>
	public static void AddConsumersFromAssemblies(
		this IRegistrationConfigurator configurator,
		params Assembly[] assembliesToScan)
	{
		if (assembliesToScan.Length == 0)
		{
			assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();
		}

		var consumerTypes = assembliesToScan.SelectMany(a => a.GetTypes())
			.Where(
				t => t is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false } &&
				     t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
			)
			.ToArray();

		foreach (var consumerType in consumerTypes)
		{
			configurator.AddConsumer(consumerType);
		}
	}

	/// <summary>
	/// Configures the message topology and receive endpoints for consumers dynamically based on the specified assemblies.
	/// This method automates the setup of Azure Service Bus topics and subscriptions for MassTransit consumers.
	/// </summary>
	/// <param name="configurator">The MassTransit service bus factory configurator.</param>
	/// <param name="context">The MassTransit bus registration context.</param>
	/// <param name="serviceName">The name of the service, used to create unique subscription names.</param>
	/// <param name="assembliesToScan">Assemblies to scan for consumers. If none specified, scans all loaded assemblies.</param>
	/// <remarks>
	/// This method performs the following key tasks:
	/// 1. Sets up a custom entity name formatter to ensure consistent naming of topics.
	/// 2. Scans assemblies for MassTransit consumer types.
	/// 3. For each consumer, creates topics and subscriptions with standardized naming conventions.
	/// 4. Configures receive endpoints for each consumer, ensuring proper message routing.
	/// 
	/// The use of reflection in this method allows for dynamic configuration without hard-coding message types,
	/// making the system more flexible and easier to maintain as new message types are added.
	/// </remarks>
	public static void ConfigureMessageTopologyAndConsumers(
		this IServiceBusBusFactoryConfigurator configurator,
		IBusRegistrationContext context,
		string serviceName,
		params Assembly[] assembliesToScan)
	{
		// If no assemblies are specified, scan all loaded assemblies.
		// This provides flexibility in configuration while ensuring all potential consumers are discovered.
		if (assembliesToScan.Length == 0)
		{
			assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();
		}

		// Set up custom entity name formatter to ensure consistent topic naming across the application.
		// This is crucial for maintaining a standardized and predictable messaging infrastructure.
		configurator.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());

		// Scan assemblies for types that implement IConsumer<T>.
		// This allows for automatic discovery of all message consumers in the application.
		var consumerTypes = assembliesToScan.SelectMany(a => a.GetTypes())
			.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
			.ToArray();

		// Convert service name to kebab-case for consistency in naming.
		var formattedServiceName = ConvertToKebabCase(serviceName);

		foreach (var consumerType in consumerTypes)
		{
			var messageTypes = GetMessageTypes(consumerType);

			foreach (var messageType in messageTypes)
			{
				// Generate standardized names for topics and subscriptions.
				// This ensures a consistent naming convention across the entire messaging system.
				var topicName = GenerateTopicName(messageType);
				//var queueName = GenerateQueueName(formattedServiceName, messageType);

				// Check for custom subscription name from attribute
				var customSubscriptionName = consumerType.GetCustomAttributes(typeof(ConsumerSubscriptionAttribute), true)
					.OfType<ConsumerSubscriptionAttribute>()
					.FirstOrDefault(attr => attr.SubscriptionName != null)?.SubscriptionName;

				var subscriptionName = customSubscriptionName ?? GenerateSubscriptionName(formattedServiceName, messageType);

				configurator.SubscriptionEndpoint(
					subscriptionName,
					topicName,
					e => { e.ConfigureConsumer(context, consumerType); }
				);

				// configurator.ReceiveEndpoint(
				// 	queueName,
				// 	e =>
				// 	{
				// 		// Configure the consumer for this endpoint.
				// 		e.ConfigureConsumer(context, consumerType);
				//
				// 		// Set concurrency limits. These should be adjusted based on the specific needs of the application.
				// 		e.PrefetchCount = 1;
				// 		e.MaxConcurrentCalls = 1;
				//
				// 		// Disable default consume topology to have full control over subscriptions.
				// 		e.ConfigureConsumeTopology = false;
				//
				// 		e.PublishFaults = false;
				//
				// 		//e.Subscribe(topicName, subscriptionName, null);
				//
				// 		// Use reflection to call the generic Subscribe method.
				// 		// This is necessary because we don't know the message type at compile time.
				// 		var subscribeMethod = typeof(IServiceBusReceiveEndpointConfigurator)
				// 			.GetMethod(
				// 				"Subscribe",
				// 				[
				// 					typeof(string), typeof(Action<IServiceBusSubscriptionConfigurator>)
				// 				]
				// 			);
				//
				// 		if (subscribeMethod != null)
				// 		{
				// 			var genericMethod = subscribeMethod.MakeGenericMethod(messageType);
				// 			genericMethod.Invoke(
				// 				e,
				// 				[
				// 					topicName, null
				// 				]
				// 			);
				// 		}
				// 		else
				// 		{
				// 			// Throw an exception if the Subscribe method is not found.
				// 			// This is crucial for diagnosing configuration issues early.
				// 			throw new InvalidOperationException($"Subscribe method not found for message type {messageType.Name}");
				// 		}
				// 	}
				// );
			}
		}
	}

	/// <summary>
	/// Custom entity name formatter to ensure consistent naming of Azure Service Bus topics.
	/// </summary>
	private class CustomEntityNameFormatter : IEntityNameFormatter
	{
		/// <summary>
		/// Formats the entity name for a given message type.
		/// </summary>
		/// <typeparam name="T">The type of the message.</typeparam>
		/// <returns>A formatted topic name based on the message type.</returns>
		public string FormatEntityName<T>()
		{
			return GenerateTopicName(typeof(T));
		}
	}

	/// <summary>
	/// Extracts the message types that a consumer handles.
	/// </summary>
	/// <param name="consumerType">The type of the consumer.</param>
	/// <returns>An enumeration of message types handled by the consumer.</returns>
	private static IEnumerable<Type> GetMessageTypes(Type consumerType)
	{
		return consumerType.GetInterfaces()
			.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
			.Select(i => i.GetGenericArguments()[0]);
	}

	/// <summary>
	/// Generates a standardized topic name for a given message type.
	/// </summary>
	/// <param name="messageType">The type of the message.</param>
	/// <returns>A kebab-case topic name with a "-topic" suffix.</returns>
	private static string GenerateTopicName(Type messageType)
	{
		// Extract the type name without namespace
		var typeName = messageType.Name;

		// Define suffixes to remove for cleaner naming
		string[] suffixesToRemove =
		[
			"Message", "Command", "Event", "Query", "Notification"
		];

		// Remove the first matching suffix found
		foreach (var suffix in suffixesToRemove)
		{
			if (!typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) continue;

			typeName = typeName[..^suffix.Length];
			break;
		}

		// Convert to kebab-case and add "-topic" suffix for clarity
		var kebabCase = ConvertToKebabCase(typeName);
		return $"{kebabCase}-topic";
	}

	// private static string GenerateQueueName(
	// 	string serviceName,
	// 	Type messageType)
	// {
	// 	var typeName = messageType.Name;
	// 	string[] suffixesToRemove =
	// 	[
	// 		"Message", "Command", "Event", "Query", "Notification", "DomainEvent"
	// 	];
	//
	// 	foreach (var suffix in suffixesToRemove)
	// 	{
	// 		if (!typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) continue;
	//
	// 		typeName = typeName[..^suffix.Length];
	// 		break;
	// 	}
	//
	// 	return $"{serviceName}-{ConvertToKebabCase(typeName)}-queue";
	// }

	/// <summary>
	/// Generates a standardized subscription name for a given message type.
	/// </summary>
	/// <param name="serviceName"></param>
	/// <param name="messageType">The type of the message.</param>
	/// <returns>A kebab-case subscription name.</returns>
	private static string GenerateSubscriptionName(
		string serviceName,
		Type messageType)
	{
		var typeName = messageType.Name;
		string[] suffixesToRemove =
		[
			"Message", "Command", "Event", "Query", "Notification", "DomainEvent"
		];

		foreach (var suffix in suffixesToRemove)
		{
			if (!typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) continue;

			typeName = typeName[..^suffix.Length];
			break;
		}

		return $"{serviceName}-{ConvertToKebabCase(typeName)}-subscription";
	}

	/// <summary>
	/// Converts a PascalCase string to kebab-case.
	/// </summary>
	/// <param name="input">The input string in PascalCase.</param>
	/// <returns>The converted string in kebab-case.</returns>
	private static string ConvertToKebabCase(string input)
	{
		if (string.IsNullOrEmpty(input))
			return input;

		var result = new StringBuilder();
		result.Append(char.ToLower(input[0]));

		for (var i = 1; i < input.Length; i++)
		{
			if (char.IsUpper(input[i]))
			{
				result.Append('-');
				result.Append(char.ToLower(input[i]));
			}
			else
			{
				result.Append(input[i]);
			}
		}

		return result.ToString();
	}
}