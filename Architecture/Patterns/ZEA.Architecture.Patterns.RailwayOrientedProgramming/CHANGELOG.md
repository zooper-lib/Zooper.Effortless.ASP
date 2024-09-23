# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [2.1.0] - 2024-04-27

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

## [2.0.0] - 2024-04-20

- Implemented a new release strategy. The format of the changelog has been updated to reflect the new strategy.

---