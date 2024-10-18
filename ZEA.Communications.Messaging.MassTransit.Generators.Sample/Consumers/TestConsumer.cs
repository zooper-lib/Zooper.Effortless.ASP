using System.Threading.Tasks;
using MassTransit;
using ZEA.Communications.Messaging.MassTransit.Attributes;
using ZEA.Communications.Messaging.MassTransit.Generators.Sample.Events;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Sample.Consumers;

[Consumer("TestTopic", "TestSubscription")]
public class TestConsumer : IConsumer<TestEventOne>
{
	public Task Consume(ConsumeContext<TestEventOne> context)
	{
		return Task.CompletedTask;
	}
}