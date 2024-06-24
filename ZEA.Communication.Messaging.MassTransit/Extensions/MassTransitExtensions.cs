using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Communication.Messaging.MassTransit.Builders;

namespace ZEA.Communication.Messaging.MassTransit.Extensions;

public static class MassTransitExtensions
{
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
	public static void AddConsumersFromAssemblies(this IRegistrationConfigurator configurator, params Assembly[] assembliesToScan)
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
	/// </summary>
	/// <param name="configurator">The MassTransit service bus factory configurator.</param>
	/// <param name="context">The MassTransit bus registration context.</param>
	/// <param name="assembliesToScan">Assemblies to scan for consumers and their subscription attributes. If none specified, scans all loaded assemblies.</param>
	public static void ConfigureMessageTopologyAndConsumers(
		this IServiceBusBusFactoryConfigurator configurator,
		IBusRegistrationContext context,
		params Assembly[] assembliesToScan)
	{
		if (assembliesToScan.Length == 0)
		{
			assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();
		}

		var consumerTypes = assembliesToScan.SelectMany(a => a.GetTypes())
			.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
			.ToArray();

		foreach (var consumerType in consumerTypes)
		{
			var messageTypes = GetMessageTypes(consumerType);

			foreach (var messageType in messageTypes)
			{
				var subscriptionName = GenerateSubscriptionName(messageType);
				configurator.ReceiveEndpoint(subscriptionName, e => { e.ConfigureConsumer(context, consumerType); });
			}
		}
	}

	private static IEnumerable<Type> GetMessageTypes(Type consumerType)
	{
		return consumerType.GetInterfaces()
			.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
			.Select(i => i.GetGenericArguments()[0]);
	}

	private static string GenerateSubscriptionName(Type messageType)
	{
		// Remove common suffixes if present
		var typeName = messageType.Name;
		string[] suffixesToRemove =
		{
			"Message", "Command", "Event", "Query", "Notification"
		};

		foreach (var suffix in suffixesToRemove)
		{
			if (!typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) continue;

			typeName = typeName[..^suffix.Length];
			break;
		}

		// Convert to kebab-case
		return string.Concat(typeName.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x : x.ToString())).ToLower();
	}
}