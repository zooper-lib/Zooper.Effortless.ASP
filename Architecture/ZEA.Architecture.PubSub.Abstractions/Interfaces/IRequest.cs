namespace ZEA.Architecture.PubSub.Abstractions.Interfaces;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;