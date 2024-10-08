using System.Threading.Tasks;
using MassTransit;
using ZEA.Communications.Messaging.MassTransit.Attributes;
using ZEA.Communications.Messaging.MassTransit.Generators.Sample.Events;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Sample.Consumers;

[Consumer("TestTopic", "TestSubscription")]
public class TestConsumer : IConsumer<TestEventOne>
{
	public async Task Consume(ConsumeContext<TestEventOne> context)
	{
		throw new System.NotImplementedException();
	}
}