namespace ZEA.Architecture.PubSub.MediatrWrapper;

public class MediatrRequestAdapter<TRequest, TResponse>(TRequest innerRequest) : MediatR.IRequest<TResponse>
	where TRequest : Abstractions.Interfaces.IRequest<TResponse>
{
	public TRequest InnerRequest { get; } = innerRequest;
}