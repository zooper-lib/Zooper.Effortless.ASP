using MassTransit;
using ZEA.Architectures.DDD.Abstractions.Interfaces;

namespace ZEA.Communications.Messaging.MassTransit.Extensions;

/// <summary>
/// Extension methods for MassTransit bus factory configurators.
/// </summary>
public static class BusFactoryConfiguratorExtensions
{
	/// <summary>
	/// Excludes base interfaces from the publish topology to prevent publishing events for them.
	/// </summary>
	/// <param name="cfg">The bus factory configurator.</param>
	public static void ExcludeBaseInterfaces(this IBusFactoryConfigurator cfg)
	{
		cfg.Publish<IIntegrationEvent>(x => x.Exclude = true);
		cfg.Publish<IDomainEvent>(x => x.Exclude = true);
		cfg.Publish<IEvent>(x => x.Exclude = true);
	}
}