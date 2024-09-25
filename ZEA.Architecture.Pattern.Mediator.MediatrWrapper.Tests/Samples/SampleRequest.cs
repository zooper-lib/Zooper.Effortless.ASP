using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests.Samples;

public class SampleRequest : IRequest<string>
{
	public string Data { get; set; }
}