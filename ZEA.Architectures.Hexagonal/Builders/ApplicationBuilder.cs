namespace ZEA.Architectures.Hexagonal.Builders;

// public class ApplicationInitializationBuilder
// {
//     private bool _isMediatorConfigured = false;
//     private Action<IServiceCollection> _configureMediator;
//
//     public ApplicationInitializationBuilder UseMediatR(Action<MediatRServiceConfiguration> configAction = null)
//     {
//         if (_isMediatorConfigured)
//             throw new InvalidOperationException("A mediator is already configured.");
//
//         _configureMediator = services =>
//         {
//             services.AddMediatR(cfg =>
//             {
//                 cfg.RegisterServicesFromAssemblies(typeof(ApplicationInitializationBuilder).Assembly);
//                 configAction?.Invoke(cfg);
//             });
//
//             // Register the ValidationBehavior for MediatR
//             services.AddTransient(
//                 typeof(IPipelineBehavior<,>),
//                 typeof(ValidationBehavior<,>)
//             );
//         };
//
//         _isMediatorConfigured = true;
//         return this;
//     }
//
//     public ApplicationInitializationBuilder UseWolverine(Action<WolverineOptions> configAction = null)
//     {
//         if (_isMediatorConfigured)
//             throw new InvalidOperationException("A mediator is already configured.");
//
//         _configureMediator = services =>
//         {
//             services.AddWolverine(cfg =>
//             {
//                 cfg.Discovery.IncludeAssembly(typeof(ApplicationInitializationBuilder).Assembly);
//                 configAction?.Invoke(cfg);
//             });
//
//             // Register any necessary behaviors or services for Wolverine
//         };
//
//         _isMediatorConfigured = true;
//         return this;
//     }
//
//     internal void ConfigureMediator(IServiceCollection services)
//     {
//         _configureMediator?.Invoke(services);
//     }
//
//     // Additional configuration methods for Validators, EventProcessors, etc.
//     public ApplicationInitializationBuilder AddValidators()
//     {
//         return this;
//     }
//
//     public ApplicationInitializationBuilder AddEventProcessors()
//     {
//         return this;
//     }
//
//     public ApplicationInitializationBuilder AddEventAppliers()
//     {
//         return this;
//     }
//
//     public ApplicationInitializationBuilder AddAutoMapper(params Assembly[] assemblies)
//     {
//         // Store assemblies for later use
//         return this;
//     }
//
//     public ApplicationInitializationBuilder AddSteps()
//     {
//         return this;
//     }
// }