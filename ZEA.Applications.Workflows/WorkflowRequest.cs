using MediatR;
using ZEA.Techniques.ADTs.Helpers;

namespace ZEA.Applications.Workflows;

/// <summary>
/// Interface that defines a workflow request that returns either a response or an error.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned on success.</typeparam>
/// <typeparam name="TError">The type of the error returned on failure.</typeparam>
public interface IWorkflowRequest<TResponse, TError> : IRequest<Either<TResponse, TError>>;

/// <summary>
/// Abstract record that implements the IWorkflowRequest interface, representing a workflow request.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned on success.</typeparam>
/// <typeparam name="TError">The type of the error returned on failure.</typeparam>
public abstract record WorkflowRequest<TResponse, TError> : IWorkflowRequest<TResponse, TError>;
