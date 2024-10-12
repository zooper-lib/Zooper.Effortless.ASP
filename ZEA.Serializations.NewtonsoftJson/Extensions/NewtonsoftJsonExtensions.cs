using Microsoft.Extensions.DependencyInjection;
using ZEA.Serializations.NewtonsoftJson.Builders;

namespace ZEA.Serializations.NewtonsoftJson.Extensions;

public static class NewtonsoftJsonExtensions
{
	/// <summary>
	/// Configures Newtonsoft.Json serialization for an ASP.NET Core application using a fluent builder API.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to add the JSON settings to.</param>
	/// <param name="configure">An optional action to further customize the JSON settings using the <see cref="NewtonsoftJsonBuilder"/>.</param>
	/// <returns>The configured <see cref="IServiceCollection"/> instance for chaining further service registrations.</returns>
	public static IServiceCollection ConfigureNewtonsoftJsonSerialization(
		this IServiceCollection services,
		Action<NewtonsoftJsonBuilder>? configure = null)
	{
		var builder = new NewtonsoftJsonBuilder(services);
		configure?.Invoke(builder);
		return builder.Build();
	}
}