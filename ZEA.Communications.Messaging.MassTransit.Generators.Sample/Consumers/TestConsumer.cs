using System.Threading.Tasks;
using MassTransit;
using ZEA.Communications.Messaging.MassTransit.Generators.Attributes;
using ZEA.Communications.Messaging.MassTransit.Generators.Sample.Events;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Sample.Consumers;

[MassTransitConsumer("TestTopic", "TestSubscription", "TestQueue")]
public class TestConsumer : IConsumer<TestEvent>
{
	public async Task Consume(ConsumeContext<TestEvent> context)
	{
		throw new System.NotImplementedException();
	}
}