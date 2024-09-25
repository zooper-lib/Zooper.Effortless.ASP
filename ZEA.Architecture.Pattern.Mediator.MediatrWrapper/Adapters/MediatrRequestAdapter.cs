namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Adapters;

public class MediatrRequestAdapter<TRequest, TResponse>(TRequest innerRequest) : MediatR.IRequest<TResponse>
	where TRequest : Pattern.Mediator.Abstractions.Interfaces.IRequest<TResponse>
{
	public TRequest InnerRequest { get; } = innerRequest;
}