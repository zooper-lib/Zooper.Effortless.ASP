using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.Hexagonal.Interfaces;

namespace ZEA.Architecture.Hexagonal.Extensions;

/// <summary>
/// Provides extension methods for registering adapters (classes implementing IPort) 
/// into the IServiceCollection with a transient lifetime.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all classes implementing IPort from the application's dependencies into the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The IServiceCollection with the registered services.</returns>
    public static IServiceCollection RegisterAdapters(this IServiceCollection services)
    {
        services.Scan(
            scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo(typeof(IPort)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }

    /// <summary>
    /// Registers all classes implementing IPort from a specific assembly into the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="assembly">The assembly to scan for classes implementing IPort.</param>
    /// <returns>The IServiceCollection with the registered services.</returns>
    public static IServiceCollection RegisterAdapters(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.Scan(
            scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IPort)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }

    /// <summary>
    /// Registers all classes implementing IPort from a set of assemblies into the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="assemblies">The assemblies to scan for classes implementing IPort.</param>
    /// <returns>The IServiceCollection with the registered services.</returns>
    public static IServiceCollection RegisterAdapters(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.Scan(
            scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IPort)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }

    /// <summary>
    /// Registers all classes implementing IPort from the assembly containing the specified type into the service collection.
    /// </summary>
    /// <typeparam name="TAssembly">The type whose assembly will be scanned for classes implementing IPort.</typeparam>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The IServiceCollection with the registered services.</returns>
    public static IServiceCollection RegisterAdapters<TAssembly>(this IServiceCollection services)
    {
        services.Scan(
            scan => scan
                .FromAssemblyOf<TAssembly>()
                .AddClasses(classes => classes.AssignableTo(typeof(IPort)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }
}