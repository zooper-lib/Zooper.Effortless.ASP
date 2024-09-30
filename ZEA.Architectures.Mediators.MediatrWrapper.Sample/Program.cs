// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZEA.Architectures.Mediators.Abstractions.Extensions;
using ZEA.Architectures.Mediators.MediatrWrapper.Extensions;
using ZEA.Architectures.Mediators.MediatrWrapper.Sample.Requests;

IServiceCollection services = new ServiceCollection();

services.AddMediator()
	.AddRequestHandlersFromAssemblies(Assembly.GetExecutingAssembly())
	.AddNotificationHandlersFromAssemblies()
	.UseMediatR()
	.Build();

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<ZEA.Architectures.Mediators.Abstractions.Interfaces.IMediator>();

var response = await mediator.SendAsync(new AddRequest(5, 10));

Console.WriteLine(response);