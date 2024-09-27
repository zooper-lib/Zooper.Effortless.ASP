namespace ZEA.Architectures.Mediators.Abstractions.Interfaces;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;