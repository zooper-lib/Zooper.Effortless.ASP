# **Discriminated Union Generator for C#**

A source generator that enables you to create discriminated unions in C# with minimal boilerplate code by leveraging
the [OneOf](https://github.com/mcintyre321/OneOf) library.

## **Overview**

This package allows you to define discriminated unions (also known as sum types or tagged unions) in C# using simple
class and method annotations. It generates the necessary code to work seamlessly with the OneOf library, enabling
pattern matching and type-safe handling of multiple variants.

## **Features**

- **Minimal Boilerplate:** Define your union types using attributes without writing repetitive code.
- **OneOf Integration:** Leverages the OneOf library for pattern matching and union type handling.
- **Variant Data Support:** Variants can carry additional data.
- **Type Safety:** Provides compile-time checks for handling all possible cases.
- **Encapsulation:** Variant classes are generated internally to prevent external instantiation.

## **Installation**

1. **Install the OneOf NuGet Package**

You need to install the [OneOf](https://www.nuget.org/packages/OneOf) library, which this generator depends on.

```shell
dotnet add package OneOf
```

2. **Install the Discriminated Union Generator Package**

Assuming the package is available on NuGet (replace YourPackageName with the actual package name):

```shell
dotnet add package ZEA.Techniques.DiscriminatedUnions.Generators
```

## **Usage**

### **Defining a Discriminated Union**

1. **Create a Partial Class**

Define a partial class and annotate it with [DiscriminatedUnion].

```csharp
[DiscriminatedUnion]
public partial class SignUpError
{
    // Variant methods will be defined here
}
```

2. **Define Variants Using [Variant] Attribute**

For each variant of your union type, define a static partial method annotated with [Variant]. These methods can have
parameters if the variant carries data.

```csharp
[DiscriminatedUnion]
public partial class SignUpError
{
    [Variant]
    public static partial SignUpError ServiceUnavailable(string message);

    [Variant]
    public static partial SignUpError InvalidCredentials();

    [Variant]
    public static partial SignUpError InternalError(Exception exception);
}
```

- Variant with Data: ServiceUnavailable and InternalError carry additional data.
- Variant without Data: InvalidCredentials carries no additional data.

3. **Build Your Project**

When you build your project, the source generator will produce the necessary code behind the scenes. The generated code
includes:

- Static methods implementing the partial methods you declared.
- Nested variant classes representing each variant.
- Constructors and properties to manage variant data.

### **Using the Discriminated Union**

### **Creating Instances**

Use the static methods to create instances of your discriminated union.

```csharp
var error1 = SignUpError.ServiceUnavailable("Scheduled maintenance");
var error2 = SignUpError.InvalidCredentials();
var error3 = SignUpError.InternalError(new Exception("Database connection failed"));
```

### **Pattern Matching**

Since the generated union type is based on the OneOf library, you can use pattern matching to handle different variants.

```csharp
string message = error1.Match(
    serviceUnavailable: s => $"Service is unavailable: {s.Message}",
    invalidCredentials: _ => "Invalid credentials provided.",
    internalError: e => $"An internal error occurred: {e.Exception.Message}"
);

Console.WriteLine(message);
// Output: Service is unavailable: Scheduled maintenance
```