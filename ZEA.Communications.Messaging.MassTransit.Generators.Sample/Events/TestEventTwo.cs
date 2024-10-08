using ZEA.Communications.Messaging.MassTransit.Attributes;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Sample.Events;

[Topic("test-topic-two")]
public sealed record TestEventTwo;