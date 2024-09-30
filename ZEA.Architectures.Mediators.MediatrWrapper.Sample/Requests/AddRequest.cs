using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Sample.Requests;

public sealed record AddRequest(double SummandOne, double SummandTwo) : IRequest<double>;