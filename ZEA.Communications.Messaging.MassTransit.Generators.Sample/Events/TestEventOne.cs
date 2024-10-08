using ZEA.Communications.Messaging.MassTransit.Attributes;

namespace ZEA.Communications.Messaging.MassTransit.Generators.Sample.Events;

[Topic("test-topic-one")]
public sealed class TestEventOne { }