using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.Pattern.Mediator.Abstractions.Builders;

namespace ZEA.Architecture.Pattern.Mediator.Abstractions.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Starts the mediator configuration builder.
	/// </summary>
	/// <param name="services">The service collection to add the mediator to.</param>
	/// <returns>A <see cref="MediatorBuilder"/> for configuring the mediator.</returns>
	public static MediatorBuilder AddMediator(this IServiceCollection services)
	{
		return new MediatorBuilder(services);
	}
}