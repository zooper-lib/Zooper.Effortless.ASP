using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architecture.Modularization.Modules;

namespace ZEA.Architecture.Modularization.Extensions;

public static class ModuleExtensions
{
	public static WebApplicationBuilder AddModules(
		this WebApplicationBuilder builder,
		params Type[] moduleTypes)
	{
		foreach (var moduleType in moduleTypes)
		{
			if (Activator.CreateInstance(moduleType) is not AppModule module) continue;

			module.ConfigureServices(builder);
			builder.Services.AddSingleton(module);
		}

		return builder;
	}

	public static WebApplication UseModules(this WebApplication app)
	{
		var modules = app.Services.GetServices<AppModule>();

		foreach (var module in modules)
		{
			module.ConfigureMiddleware(app);
		}

		return app;
	}
}