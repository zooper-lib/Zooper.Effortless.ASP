using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Sample.Requests;

public sealed record SubtractRequest(double Minuend, double Subtrahend) : IRequest<double>;