# Domain-Driven Design (DDD) with ZEA

## Domain Event Serialization and Deserialization

### Overview

This section explains how to use the domain event serialization and deserialization functionality provided by the
DomainEventSerializer and DomainEventTypeResolver classes. These utilities simplify the process of converting domain
events to and from JSON format, making it easier to store, transmit, and handle events in your application.

### Key Components

- `DomainEventSerializer`: A utility class that handles the serialization and deserialization of domain events.
- `DomainEventTypeResolver`: A utility class that resolves the correct event type based on a given event name, ensuring
  that the correct class is instantiated during deserialization.
- `DomainEventNameAttribute`: An attribute used to annotate domain event classes with a unique event name, which is
  essential for the event type resolution process.

### Using DomainEventNameAttribute

#### Annotating Domain Events

Each domain event class should be annotated with the `DomainEventNameAttribute`. This attribute assigns a unique name to
the event, which is critical for the deserialization process. The event name specified in this attribute is what the
`DomainEventSerializer` and `DomainEventTypeResolver` use to identify the correct event type.

```csharp
[DomainEventName("UserCreatedEvent")]
public class UserCreatedEvent : IDomainEvent
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

### Using DomainEventSerializer

#### 1. Serializing Domain Events

To serialize a domain event, you need to create an instance of the DomainEventSerializer and pass your domain event
to the SerializeEvent method. This will convert your event into a JSON-encoded byte array, which can then be stored
or transmitted as needed.

```csharp
var userCreatedEvent = new UserCreatedEvent
{
    UserId = Guid.NewGuid(),
    Username = "johndoe",
    Email = "johndoe@example.com"
};

var serializer = new DomainEventSerializer();
byte[] serializedEvent = serializer.SerializeEvent(userCreatedEvent);
```

#### 2. Deserializing Domain Events

To deserialize a domain event, use the DeserializeEvent method of the DomainEventSerializer. You must provide the event
name (which should match the DomainEventNameAttribute of the corresponding class) and the JSON-encoded byte array.

```csharp
string eventName = "UserCreatedEvent";
byte[] eventData = serializedEvent;

IDomainEvent? deserializedEvent = serializer.DeserializeEvent(eventName, eventData);

if (deserializedEvent is UserCreatedEvent userCreated)
{
    Console.WriteLine($"User Created: {userCreated.Username}, {userCreated.Email}");
}
```

### Using DomainEventTypeResolver

The DomainEventTypeResolver helps resolve the appropriate domain event type based on the event name. This is
particularly useful in scenarios where events need to be dynamically instantiated based on their name.

#### Resolving Event Types

You can resolve event types by passing an event name to the ResolveEventType method. Optionally, you can specify the
assemblies to search, or it will default to searching all loaded assemblies.

```csharp
var eventType = DomainEventTypeResolver.ResolveEventType("UserCreatedEvent");
if (eventType != null)
{
    Console.WriteLine($"Resolved Event Type: {eventType.Name}");
}
```

If you want to limit the search to specific assemblies:

```csharp
var eventType = DomainEventTypeResolver.ResolveEventType("UserCreatedEvent", Assembly.GetExecutingAssembly());
```

### Example Workflow

Here’s an example that combines serialization, deserialization, and event type resolution:

```csharp
[DomainEventName("OrderPlacedEvent")]
public class OrderPlacedEvent : IDomainEvent
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}

// Create and serialize an event
var orderPlacedEvent = new OrderPlacedEvent
{
    OrderId = Guid.NewGuid(),
    ProductName = "Laptop",
    Price = 1200.00m
};

var serializer = new DomainEventSerializer();
byte[] serializedOrder = serializer.SerializeEvent(orderPlacedEvent);

// Deserialize the event using its name
string eventName = "OrderPlacedEvent";
IDomainEvent? deserializedOrder = serializer.DeserializeEvent(eventName, serializedOrder);

if (deserializedOrder is OrderPlacedEvent orderPlaced)
{
    Console.WriteLine($"Order Placed: {orderPlaced.ProductName}, ${orderPlaced.Price}");
}
```