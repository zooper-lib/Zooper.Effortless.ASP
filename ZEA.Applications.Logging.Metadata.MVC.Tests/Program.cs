using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Extensions;
using ZEA.Applications.Logging.Metadata.MVC.Tests;
using ZEA.Applications.Logging.Metadata.MVC.Tests.Adaptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.AddNewtonsoftJson(
		options =>
		{
			options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
			options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
		}
	);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(
	options =>
	{
		options.Cookie.HttpOnly = true;
		options.Cookie.IsEssential = true;
	}
);

// Register the fake authentication scheme
builder.Services.AddAuthentication(
		options =>
		{
			options.DefaultAuthenticateScheme = "FakeAuthentication";
			options.DefaultChallengeScheme = "FakeAuthentication";
		}
	)
	.AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("FakeAuthentication", options => { });

// Register request metadata services
builder.Services.AddRequestMetadata<RequestMetadata, CustomRequestMetadataService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection()
	.UseRouting()
	.UseSession()
	.UseAuthentication()
	.UseAuthorization()
	.UseRequestMetadata<RequestMetadata>()
	.UseEndpoints(
		endpoints => { endpoints.MapControllers(); }
	);

app.Run();