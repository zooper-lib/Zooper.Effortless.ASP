namespace ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;