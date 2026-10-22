# Pure.Authorization.OpenFGA.Model.Conditions

`IBool` condition types over OpenFGA authorization models for the **Pure** ecosystem.

[![.NET build & test](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/build-and-test.yml)
[![Build and Deploy](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.Authorization.OpenFGA.Model.Conditions)](https://www.nuget.org/packages/Pure.Authorization.OpenFGA.Model.Conditions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> **Status:** the API is coming soon. The repository is bootstrapped; the condition types will land once `Pure.Authorization.OpenFGA.Model.Abstractions` is published.

## Overview

`Pure.Authorization.OpenFGA.Model.Conditions` will provide composable `IBool` condition types that check OpenFGA authorization models for internal consistency, and check subjects and relationship tuples for conformance to a model. Each condition is a lazy, immutable value object that implements `IBool` from `Pure.Primitives.Abstractions`. No condition throws: an inconsistent model or a non-conforming subject or tuple evaluates to `false`.

## Planned conditions

### Model consistency

- **Rewrite nodes reference existing relations** — every relation referenced by a userset rewrite (computed userset, tuple-to-userset) is defined on the relevant type.
- **Tupleset relations are direct-only** — the tupleset relation of a tuple-to-userset rewrite is assignable only directly, not through a rewrite.
- **Conditional forms are not nested** — a conditional subject form does not wrap another conditional form.
- **Referenced conditions exist** — every condition named by a conditional subject form is defined in the model.

### Conformance to a model

- **Known type** — an object or subject type is defined in the model.
- **Relation of a type** — a relation is defined on the given type.
- **Assignable relation for a subject form** — a relation accepts the given subject form (direct type, wildcard, userset, or conditional) as a direct assignment.
- **Valid identifiers** — object and subject identifiers are well formed.

## Design Principles

- **IBool results** — every condition is an `IBool`, keeping results composable within the Pure type system.
- **Lazy evaluation** — conditions are evaluated only when `.BoolValue` is accessed.
- **Non-throwing** — invalid input produces `false`, never an exception.
- **AOT-compatible** — no reflection; fully compatible with Native AOT compilation.

## Dependencies

- [`Pure.Authorization.OpenFGA.Model.Abstractions`](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Abstractions) — OpenFGA authorization model domain interfaces (planned)
- [`Pure.Primitives.Abstractions`](https://github.com/kudima03/Pure.Primitives.Abstractions) — `IBool` interface
