## TypeSerializationBinder<TType>

### Overview

The `TypeSerializationBinder<TType>` is a custom serialization binder designed to handle scenarios where objects need to
be serialized and deserialized without being tightly coupled to specific assembly names. It maps type names to their
corresponding Type objects at runtime, which is especially useful when dealing with multiple versions of an assembly or
in distributed systems where the exact assembly name and version might vary.

This binder ensures that types implementing or deriving from a common base type (TType) can be serialized and
deserialized by name, rather than being tied to a specific assembly, making the serialization process more flexible and
robust.

### Features

- Type Name-Based Serialization: Serializes objects by only recording their type names, ignoring assembly names and
  versions.
- Cross-Assembly Deserialization: Deserializes objects based on their type name, making it possible to deserialize
  objects
  even when the assembly name has changed or the object comes from a different version of the application.
- Automatic Type Caching: Automatically caches all types in the application domain that implement TType and are neither
  abstract classes nor interfaces.
- Exception Handling: Throws a clear exception when the specified type name cannot be found, aiding in debugging
  serialization issues.

### Use Cases

The `TypeSerializationBinder<TType>` is particularly useful in:

- Versioned APIs: When maintaining backward compatibility across different versions of your application, where assembly
  names might change, but the core type structure remains the same.
- Distributed Systems: In scenarios where types are serialized in one service or application and deserialized in
  another, potentially running different versions of the same assembly.
- Loose Coupling: When you need to serialize objects but want to avoid tight coupling between their specific assembly
  names and versions, improving the flexibility of your system.

### Example Usage

```csharp
// Create an instance of the binder for a specific base type or interface
var binder = new TypeSerializationBinder<IMyInterface>();

// Serializing an object
var myObject = new MyConcreteClass();
var serializedData = JsonConvert.SerializeObject(myObject, new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Objects,
    SerializationBinder = binder
});

// Deserializing an object
var deserializedObject = JsonConvert.DeserializeObject<IMyInterface>(serializedData, new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Objects,
    SerializationBinder = binder
});
```

### Exception Handling

If the BindToType method cannot find the type name in its cache, a TypeLoadException is thrown with a detailed message,
aiding in the identification of serialization/deserialization issues.

### Considerations

This class is optimized for scenarios where types are well-known and implement or derive from TType. If your type names
are ambiguous or prone to conflicts, additional strategies for disambiguation may be needed.
Ensure that the type name is unique within the application domain, as only the name (and not the namespace or assembly)
is used to identify types.

## MultiTypeSerializationBinder

### Overview

The `MultiTypeSerializationBinder` is an extension of the `TypeSerializationBinder` designed to handle more complex
scenarios where objects from multiple type hierarchies need to be serialized and deserialized. It allows you to provide
multiple base types (or interfaces) across different assemblies, making it ideal for systems where different types, such
as IEvent, IMetadata, etc., need to be handled simultaneously.

Like the TypeSerializationBinder, this binder maps type names to their corresponding Type objects at runtime, ensuring
flexibility in scenarios where assembly names or versions might differ.

### Features

- Multiple Base Types: Supports multiple base types or interfaces, allowing types from different hierarchies to be
  serialized and deserialized.
- Type Name-Based Serialization: Serializes objects by recording only their type names, ignoring assembly names and
  versions.
- Cross-Assembly Deserialization: Supports deserialization of objects based on their type name across multiple
  assemblies,
  making it possible to deserialize objects even if the assembly name or version has changed.
- Automatic Type Caching: Automatically caches all types that implement or derive from the specified base types across
  the
  given assemblies.
- Exception Handling: Provides clear exceptions when the specified type name cannot be found in the cache, aiding in
  debugging serialization issues.

### Example Usage

```csharp
// Create an instance of the binder for multiple base types (e.g., IEvent, IMetadata)
var binder = new MultiTypeSerializationBinder(new[] { typeof(IEvent), typeof(IMetadata) });

// Serializing an object
var myObject = new MyConcreteEvent();
var serializedData = JsonConvert.SerializeObject(myObject, new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Objects,
    SerializationBinder = binder
});

// Deserializing an object
var deserializedObject = JsonConvert.DeserializeObject<IEvent>(serializedData, new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Objects,
    SerializationBinder = binder
});
```

### Use Cases

The `MultiTypeSerializationBinder` is particularly useful in:

- Event Sourcing Systems: Where different event types and metadata need to be handled seamlessly across multiple
  assemblies.
- Distributed Systems: In scenarios where types from different type hierarchies (e.g., IEvent, IMetadata) are serialized
  in one service and deserialized in another.
- Versioned APIs: Useful for maintaining backward compatibility across different versions of your application, where
  assembly names might change, but the core type structure remains the same.

### Exception Handling

If the BindToType method cannot find the type name in its cache, a TypeLoadException is thrown with a detailed message,
making it easier to identify serialization and deserialization issues.

### Considerations

Ensure that the assemblies provided are properly loaded when instantiating the binder. If none are provided, it defaults
to scanning all assemblies in the current application domain.
Make sure that the type names are unique within the application domain, as only the name (and not the namespace or
assembly) is used to identify types.