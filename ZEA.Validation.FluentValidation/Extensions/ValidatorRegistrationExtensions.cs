using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global

namespace ZEA.Validation.FluentValidation.Extensions;

// ReSharper disable once UnusedType.Global
public static class ValidatorRegistrationExtensions
{
	public static IServiceCollection RegisterPipelineBehavior(this IServiceCollection services)
	{
		services.AddTransient(
			typeof(IPipelineBehavior<,>),
			typeof(ValidationBehavior<,>)
		);

		return services;
	}

	public static IServiceCollection RegisterValidatorsFromAssemblies(
		this IServiceCollection services,
		params Assembly[] assemblies)
	{
		var validatorType = typeof(IValidator<>);
		var validatorTypes = new List<Type>();

		foreach (var assembly in assemblies)
		{
			var typesInAssembly = assembly.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract)
				.Where(
					t => IsSubclassOfRawGeneric(
						validatorType,
						t
					)
				)
				.ToList();

			validatorTypes.AddRange(typesInAssembly);
		}

		foreach (var type in validatorTypes)
		{
			var genericType = type.BaseType.GetGenericArguments()[0];
			services.AddTransient(
				typeof(IValidator<>).MakeGenericType(genericType),
				type
			);
		}

		return services;
	}

	private static bool IsSubclassOfRawGeneric(
		Type generic,
		Type toCheck)
	{
		if (toCheck == null || toCheck == typeof(object))
		{
			return false;
		}

		// Check all interfaces at the current level.
		if (toCheck.GetInterfaces()
		    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == generic))
		{
			return true;
		}

		// Check the current level's base type if it's a generic type and matches the generic type definition.
		if (toCheck.IsGenericType && toCheck.GetGenericTypeDefinition() == generic)
		{
			return true;
		}

		// Recursively check the base type.
		return IsSubclassOfRawGeneric(
			generic,
			toCheck.BaseType
		);
	}
}