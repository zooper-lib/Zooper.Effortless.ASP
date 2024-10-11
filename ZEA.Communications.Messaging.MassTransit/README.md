# ChannelAttribute Usage Guide

The ChannelAttribute is a custom attribute designed to associate a message class with a specific messaging channel, such
as a topic, exchange, or queue. This attribute enables centralized and consistent configuration of messaging entities
across different messaging systems like Azure Service Bus, RabbitMQ, or others supported by MassTransit.

## Purpose

Decoupling Message Classes from Infrastructure: By specifying the channel name directly on the message class, you
decouple your code from hard-coded infrastructure configurations.
Consistency Across Transports: The ChannelAttribute provides a transport-agnostic way to define messaging channels,
making your code more flexible and easier to maintain.
Facilitating Code Generation: When used with a source generator, the ChannelAttribute allows automatic generation of
messaging entity configurations, reducing boilerplate code and potential errors.

## How to Use

### Annotate Your Message Classes

Apply the ChannelAttribute to your message classes, specifying the channel name that the message should be associated
with.

```csharp
[Channel("order-updates")]
public class OrderUpdatedEvent
{
    public Guid OrderId { get; set; }
    public DateTime UpdatedAt { get; set; }
    // Other properties...
}
```