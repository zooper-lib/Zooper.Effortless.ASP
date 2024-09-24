using ZEA.Architecture.PubSub.Abstractions.Interfaces;

namespace ZEA.Architecture.PubSub.MediatR.Tests.Samples;

public class SampleRequest : IRequest<string>
{
	public string Data { get; set; }
}