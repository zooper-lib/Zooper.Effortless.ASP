using ZEA.Architectures.Mediators.Abstractions.Interfaces;

namespace ZEA.Architectures.Mediators.MediatrWrapper.Tests.Samples;

public class SampleNotification(string message) : INotification
{
	public string Message { get; set; } = message;
}