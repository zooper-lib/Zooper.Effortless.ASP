using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Serializations.Abstractions.Interfaces;

namespace ZEA.Serializations.Abstractions.Extensions;

public static class TypeConverterExtensions
{
	/// <summary>
	///     Adds all <see cref="TypeConverter" />s in the given assemblies to the <see cref="TypeDescriptor" />
	/// </summary>
	/// <param name="services"></param>
	/// <param name="assemblies"></param>
	public static IServiceCollection AddTypeConverters(
		this IServiceCollection services,
		params Assembly[] assemblies)
	{
		if (!assemblies.Any()) assemblies = AppDomain.CurrentDomain.GetAssemblies();

		foreach (var assembly in assemblies)
		foreach (var type in assembly.GetTypes())
		{
			if (type is not { IsClass: true, IsAbstract: false } || !typeof(TypeConverter).IsAssignableFrom(type)) continue;


			foreach (var it in type.GetInterfaces())
			{
				if (!it.IsGenericType || it.GetGenericTypeDefinition() != typeof(ITypeConverterFor<>)) continue;

				var targetType = it.GetGenericArguments()[0];
				TypeDescriptor.AddAttributes(
					targetType,
					new TypeConverterAttribute(type)
				);
			}
		}

		return services;
	}
}