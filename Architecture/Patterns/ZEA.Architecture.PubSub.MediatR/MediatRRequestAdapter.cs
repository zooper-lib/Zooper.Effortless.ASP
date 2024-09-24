namespace ZEA.Architecture.PubSub.Mediat;

public class MediatRRequestAdapter<TRequest, TResponse> : MediatR.IRequest<TResponse>
	where TRequest : Abstractions.Interfaces.IRequest<TResponse>
{
	public TRequest InnerRequest { get; }

	public MediatRRequestAdapter(TRequest innerRequest)
	{
		InnerRequest = innerRequest;
	}
}