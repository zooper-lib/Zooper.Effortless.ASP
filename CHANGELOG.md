# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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