﻿using Microsoft.AspNetCore.Builder;

namespace ZEA.Architectures.Modularization.Modules;

public abstract class AppModule
{
	public virtual void ConfigureServices(WebApplicationBuilder builder) { }
	public virtual void ConfigureMiddleware(WebApplication app) { }
}