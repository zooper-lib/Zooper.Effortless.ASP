using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZEA.Api.Swagger.Filters;

namespace ZEA.Api.Swagger.Extensions;

[UsedImplicitly]
public static class SwaggerExtensions
{
	[UsedImplicitly]
	public static IServiceCollection AddSwaggerWithAllDocumentFilters(
		this IServiceCollection services,
		string version,
		string title,
		string description = "",
		Uri? termsOfUseUrl = null,
		params Assembly[] assemblies)
	{
		if (assemblies.Length == 0)
		{
			assemblies = AppDomain.CurrentDomain.GetAssemblies();
		}

		// Add Swagger services
		services.AddSwaggerGen(
			c =>
			{
				// Add the ExamplesFilter to generate example responses
				c.ExampleFilters();

				c.SwaggerDoc(
					version,
					new()
					{
						Title = title,
						Version = version,
						Description = description,
						TermsOfService = termsOfUseUrl
					}
				);

				c.AddSecurityDefinition(
					"Bearer",
					new()
					{
						In = ParameterLocation.Header,
						Description = "Please enter a valid token",
						Name = "Authorization",
						Type = SecuritySchemeType.Http,
						BearerFormat = "JWT",
						Scheme = "Bearer"
					}
				);
				c.AddSecurityRequirement(
					new()
					{
						{
							new()
							{
								Reference = new()
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							},
							Array.Empty<string>()
						}
					}
				);
				c.MapType<TimeSpan>(
					() => new()
					{
						Type = "string",
						Example = new OpenApiString("PTH0M0S0")
					}
				);

				// Automatically register all classes implementing IDocumentFilter
				ApplyAutoDocumentFilters(
					c,
					assemblies
				);

				// Automatically register all classes implementing ISchemaFilter
				ApplySchemaFilters(
					c,
					assemblies
				);
			}
		);

		// Add the ExamplesFilter to generate example responses
		services.AddSwaggerExamplesFromAssemblies(assemblies);

		return services;
	}

	private static void ApplyAutoDocumentFilters(
		SwaggerGenOptions options,
		IEnumerable<Assembly> assemblies)
	{
		var documentFilters = GetInstancesOf<IDocumentFilter>(assemblies);
		foreach (var filter in documentFilters) options.DocumentFilter<AutoDocumentFilter>(filter);
	}

	private static void ApplySchemaFilters(
		SwaggerGenOptions options,
		IEnumerable<Assembly> assemblies)
	{
		var schemaFilterTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
			.Where(type => typeof(ISchemaFilter).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
			.ToList();

		foreach (var type in schemaFilterTypes)
		{
			var method = typeof(Microsoft.Extensions.DependencyInjection.SwaggerGenOptionsExtensions).GetMethod(
					nameof(Microsoft.Extensions.DependencyInjection.SwaggerGenOptionsExtensions.SchemaFilter),
					BindingFlags.Public | BindingFlags.Static
				)
				?.MakeGenericMethod(type);

			try
			{
				method!.Invoke(
					null,
					[options, Array.Empty<object>()]
				);
			}
			catch (Exception ex)
			{
				// Handle or log the exception as needed
				Console.WriteLine($"Error applying schema filter for type {type.Name}: {ex.Message}");
			}
		}
	}

	private static IEnumerable<T> GetInstancesOf<T>(IEnumerable<Assembly> assemblies)
	{
		return assemblies.SelectMany(a => a.GetTypes())
			.Where(t => typeof(T).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
			.Select(t => (T)Activator.CreateInstance(t)!);
	}
}