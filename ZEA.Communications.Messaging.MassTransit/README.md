# Using MassTransit Builders for Messaging Integration

## Introduction

This guide will walk you through integrating MassTransit into your .NET application using our custom builders. You'll
learn how to:

- Add messaging services and publishers.
- Register consumers, including those generated via source generators.
- Configure Azure Service Bus or RabbitMQ as your message broker.
- Customize serialization settings.
- Exclude base interfaces from publishing.
- Configure retry policies and dead-lettering.
- Apply additional transport-specific configurations.

## Step-by-Step Implementation

### 1. Add Messaging Services with MassTransit

First, you need to add the messaging services to your application's service collection. We have a method
AddMessagingWithMassTransit that accepts a configuration action.

```csharp
builder.Services.AddMessagingWithMassTransit(massTransitBuilder =>
{
    // Configuration will go here
});
```

### 2. Add Publisher Services

To enable message publishing, add the publisher services to the MassTransit builder.

```csharp
massTransitBuilder.AddPublisher();
```

#### Explanation:

- `AddPublisher()`: Registers the `IMessagePublisher` and `IEventPublisher` services, allowing your application to
  publish messages and events to the message broker.

### 3.Register Consumers

Consumers are classes that handle incoming messages. You can register them using the `AddConsumers` method. Use our
source generator to generate consumers automatically.

```csharp
massTransitBuilder.AddConsumers(configurator =>
{
    // Use your generated method to register consumers
    configurator.AddMassTransitConsumers();
});
```

#### Explanation:

- `AddConsumers`: Accepts an action where you can register your consumers.
- `configurator.AddMassTransitConsumers()`: This is a generated method from your source code that registers all
  consumers.
  Replace this with your actual consumer registration logic if you don't use source generators.

### 4. Configure the Message Broker

Choose and configure your message broker (Azure Service Bus or RabbitMQ). In this example, we'll focus on Azure Service
Bus.

```csharp
massTransitBuilder.UseAzureServiceBus(connectionString, configure =>
{
// Broker configuration will go here
});
```

#### Explanation:

- `UseAzureServiceBus`: Configures MassTransit to use Azure Service Bus as the message broker.
- `connectionString`: The connection string to your Azure Service Bus namespace.
- `configure`: An action to configure transport-specific settings.

### 5. Customize Serialization Settings

MassTransit uses JSON.Net serialization by default. You can customize the serialization settings to meet your
requirements.

```csharp
configure.UseNewtonsoftJson(jsonSerializerSettings =>
{
    jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
    return jsonSerializerSettings;
});
```

### 6. Exclude Base Interfaces from Publishing

When publishing messages, MassTransit can include base interfaces. This means in MassTransit will create a topic Azure
Service Bus with the base interfaces. Using this library, there will be topics for `IEvent` `IDomainEvent` and
`IIntegrationEvent`. You can exclude them if you don't want this behavior.

```csharp
configure.ExcludeBaseInterfacesFromPublishing(true);
```

#### Explanation:

- `ExcludeBaseInterfacesFromPublishing(true)`: Instructs MassTransit not to publish messages for base interfaces of your
  message contracts. This can reduce the number of message types on the broker and simplify your messaging topology.

### 7. Configure the Bus

You can provide additional configurations for the bus, such as configuring subscriptions and channels.

```csharp
configure.ConfigureBus((configurator, context) =>
{
    configurator.ConfigureSubscriptions(context);
    configurator.ConfigureChannels();
});
```

### 8. Configure Retry Policy

Configure how MassTransit retries message delivery in case of failures.

```csharp
configure.UseMessageRetry(retryConfigurator =>
{
    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
});
```

#### Explanation:

- `UseMessageRetry`: Configures the retry policy for message delivery.
- `retryConfigurator.Interval(3, TimeSpan.FromSeconds(5))`: Specifies that MassTransit should retry message delivery up
  to 3 times, with a 5-second interval between attempts.

### 9. Configure Dead-Lettering

Set up dead-lettering to handle messages that cannot be delivered successfully after retries.

```csharp
configure.EnableDeadLetteringOnMessageExpiration(true);
```

#### Explanation:

- `EnableDeadLetteringOnMessageExpiration(true)`: Enables dead-lettering for messages that expire or exceed the maximum
  delivery attempts.
- ##### For Azure Service Bus:
    - `configure.SetMaxDeliveryCount(5);`: Optionally, you can set the maximum number of delivery attempts before a
      message is dead-lettered.