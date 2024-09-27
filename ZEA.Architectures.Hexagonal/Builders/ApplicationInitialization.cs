namespace ZEA.Architectures.Hexagonal.Builders;

// public static class ApplicationInitialization
// {
// 	public static void ConfigureApplicationLayer(
// 		IServiceCollection services,
// 		Action<ApplicationInitializationBuilder> configureBuilder)
// 	{
// 		var builder = new ApplicationInitializationBuilder();
// 		configureBuilder(builder);
//
// 		// Configure Mediator
// 		builder.ConfigureMediator(services);
//
// 		// Register Validators
// 		services.Scan(
// 			scan => scan
// 				.FromAssemblyOf<ApplicationInitializationBuilder>()
// 				.AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
// 				.AsImplementedInterfaces()
// 				.WithTransientLifetime()
// 		);
//
// 		// Register EventProcessors
// 		services.Scan(
// 			scan => scan
// 				.FromAssemblyOf<ApplicationInitializationBuilder>()
// 				.AddClasses(classes => classes.AssignableTo(typeof(IDomainEventProcessor<>)))
// 				.AsImplementedInterfaces()
// 				.WithTransientLifetime()
// 		);
//
// 		// Register EventAppliers
// 		services.Scan(
// 			scan => scan
// 				.FromAssemblyOf<ApplicationInitializationBuilder>()
// 				.AddClasses(classes => classes.AssignableTo(typeof(IAggregateEventApplier<,>)))
// 				.AsImplementedInterfaces()
// 				.WithTransientLifetime()
// 		);
//
// 		// Register AutoMapper
// 		services.AddAutoMapper(typeof(ApplicationInitializationBuilder).Assembly);
//
// 		// Register Steps
// 		services.Scan(
// 			scan => scan
// 				.FromAssemblyOf<ApplicationInitializationBuilder>()
// 				.AddClasses(classes => classes.AssignableTo(typeof(IStep)))
// 				.AsImplementedInterfaces()
// 				.WithTransientLifetime()
// 		);
// 	}
// }