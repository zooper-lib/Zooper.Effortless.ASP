using ZEA.Architecture.Pattern.Mediator.Abstractions.Interfaces;

namespace ZEA.Architecture.Pattern.Mediator.MediatrWrapper.Tests.Samples;

public class SampleNotification(string message) : INotification
{
	public string Message { get; set; } = message;
}