using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Tests.Samples;

public class SampleRequest : IRequest<string>
{
	public string Data { get; set; }
}