# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

[2.8.1] - 2025.03.26

- Made the setters of the ID field of `EntityClass` and `EntityRecord` protected set.

---

[2.8.0] - 2025.01.11

- Renamed some methods of the `WorkflowBuilder` in the `ZEA.Applications.Workflows` project to make them more consistent.
- Added interfaces `IWorkflowStep` and `IWorkflowCondition` to the `ZEA.Applications.Workflows` project.

---

[2.7.0] - 2025.01.10

- Added pre-steps for the `WorkflowBuilder` in the `ZEA.Applications.Workflows` project.

---

[2.6.0] - 2025.01.02

- Implemented a Workflow Builder for the `ZEA.Applications.Workflows` project which is a lightweight builder that allows
  you to define workflows in a more fluent way.

---

[2.5.0] - 2024.11.27

- Added implicit operators to the `Either` class to make it easier to work with.
- Fixed a bug in the Discriminated Unions generator that caused the generated code to be invalid.

---

[2.4.0] - 2024.11.27

Added a new project: **ZEA.Techniques.DiscriminatedUnions.Generators** which makes it easy to define and work with discriminated
unions.

---

[2.3.0] - 2024.11.01

### Added

- **ZEA.Applications.Workflows**
    - Added base classes for executing a behavior before the workflow starts.

---

[2.2.4]

### Fixed

- **ZEA.Serializations.NewtonsoftJson**
    - Fixed the `MultiTypeSerializationBinder`.

---

[2.2.3] - 2024.10.18

### Added

- **ZEA.Techniques.RailwayOrientedProgramming**
    - Added extension methods to improve the readability of the Railway Oriented Programming pattern.

- **ZEA.Applications.Workflows**
    - New project that provides a framework for defining and executing workflows.

### Updated

- **ZEA.Communications.Messaging.MassTransit.Generators.***
    - Updated all the generator projects to use the newer `IIncrementalGenerator` interface.

---

[2.2.2] - 2024.10.16

### Fixed

- **ZEA.Communications.Messaging.MassTransit.Generators.AzureServiceBus**
    - Fixed a wrong namespace

---

## [2.2.0] - 2024-09-24

### Added

- **ZEA.Architecture.Pattern.Mediator.***
    - Introduced new packages which serve as a wrapper for different Mediator libraries.
- **ZEA.Communications.Messaging.Abstractions**
    - Added the possibility to add metadata to a publish command.
- **ZEA.Applications.Logging.Metadata.Abstractions**
- **ZEA.Applications.Logging.Metadata.MVC**

### Deprecated

- **ZEA.Architecture.Pattern.Mediator.Abstractions.Responses**
    - Marked all the response classes as `[Obsolete]` to encourage migration to the new `LogicalError` classes.
- **ZEA.Communications.Messaging.MassTransit**
    - Marked the `RabbitMqBuilder` and `AzureServiceBusBuilder` classes as `[Obsolete]` to encourage migration to the
      new `MassTransitBuilder` class.

### Renamed

- **ZEA.Architecture.PubSub.***
    - Renamed the namespace from `ZEA.Architecture.PubSub` to `ZEA.Architecture.Pattern.Mediator`.

- **ZEA.Architecture.Patterns.***
    - Renamed the namespace from `ZEA.Architecture.Patterns` to `ZEA.Architecture.Pattern` for consistency.

---

## [2.1.0] - 2024-09-20

### Added

- **Nested Interfaces:**
    - Introduced the following nested interfaces within `IEitherOneOfStep` for enhanced organization and clarity:
        - `Generator<TOutput, TError>`: For steps that generate a result without requiring input.
        - `Processor<TInput, TOutput, TError>`: For steps that process input data to produce an output.
        - `Transformer<T, TError>`: For steps that transform data while maintaining the same input and output type.

### Deprecated

- **Old Interfaces:**
    - Marked the following interfaces as `[Obsolete]` to encourage migration to the new nested interfaces:
        - `IEitherOneOfStep<TOutput, TError>`
        - `IEitherOneOfStep<in TInput, TOutput, TError>`

### Migration

- **Guidance for Developers:**
    - Transition from the deprecated interfaces to the new nested interfaces (`Generator`, `Processor`, `Transformer`)
      to take advantage of improved clarity and maintainability.
    - Update implementations and usages to reference the new nested interfaces accordingly.

### Documentation

- **Updates:**
    - Revised XML summaries and documentation to reflect the introduction of the new nested interfaces and the
      deprecation of the old interfaces.
    - Provided clear guidance on the purpose and usage of each new interface to facilitate smooth migration.

---

## [2.0.0] - 2024-09-19

- Implemented a new release strategy. The format of the changelog has been updated to reflect the new strategy.

---