# Pure.Authorization.OpenFGA.Model.Conditions

`IBool` condition types over OpenFGA authorization models for the **Pure** ecosystem.

[![.NET build & test](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/build-and-test.yml)
[![Build and Deploy](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Conditions/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/Pure.Authorization.OpenFGA.Model.Conditions)](https://www.nuget.org/packages/Pure.Authorization.OpenFGA.Model.Conditions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

`Pure.Authorization.OpenFGA.Model.Conditions` provides composable `IBool` condition types over the interfaces of [`Pure.Authorization.OpenFGA.Model.Abstractions`](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Abstractions). They check identifiers against OpenFGA's syntax, check an authorization model for internal consistency, and check a relationship tuple for conformance to a model. Each condition is a lazy, immutable value object that implements `IBool` from `Pure.Primitives.Abstractions`.

Every rule comes from OpenFGA's own validation, and each condition cites where OpenFGA enforces it. The citations point at [`openfga/api@c0650ce`](https://github.com/openfga/api/tree/c0650ce2169bcb39a72d73841fb531c200e32248) and [`openfga/openfga@v1.22.0`](https://github.com/openfga/openfga/tree/v1.22.0).

```csharp
IBool valid = new ValidAuthorizationModelCondition(model);

IBool assignable = new AssignableTupleCondition(model, tuple);

if (valid.BoolValue && assignable.BoolValue)
{
    // The model passes every rule listed below, and the tuple conforms to it.
}
```

## Conditions

Lengths given in code points count Unicode code points, as OpenFGA's Go regular expressions do. In those patterns `\s` is Go's `[\t\n\f\r ]`. A *control character* is U+0000 to U+001F or U+007F to U+009F (Go's `unicode.IsControl`).

### Identifier syntax

| Condition | Holds when | OpenFGA source |
|---|---|---|
| `ValidTypeNameCondition(IString)` | 1 to 254 code points, no `:`, `#`, `@` or `\s`, and not the reserved `self` or `this` | `openfga/api` [`openfga/v1/authzmodel.proto`](https://github.com/openfga/api/blob/c0650ce2169bcb39a72d73841fb531c200e32248/openfga/v1/authzmodel.proto) `TypeDefinition.type`; `openfga/openfga` [`pkg/typesystem/typesystem.go`](https://github.com/openfga/openfga/blob/v1.22.0/pkg/typesystem/typesystem.go) `validateNames` |
| `ValidRelationNameCondition(IString)` | 1 to 50 code points, no `:`, `#`, `@` or `\s`, and not the reserved `self` or `this` | `openfga/api` `authzmodel.proto` `TypeDefinition.relations` keys; `openfga/openfga` `pkg/typesystem/typesystem.go` `validateNames` |
| `ValidConditionNameCondition(IString)` | 1 to 50 code points, no `:`, `#`, `@` or `\s` | `openfga/api` `authzmodel.proto` `AuthorizationModel.conditions` keys and `Condition.name` |
| `ValidObjectCondition(IObject)` | the type and the id are not empty and have no `:`, `#`, space or control character; the id is not `*`; `type:id` is at most 256 code points | `openfga/api` [`openfga/v1/openfga.proto`](https://github.com/openfga/api/blob/c0650ce2169bcb39a72d73841fb531c200e32248/openfga/v1/openfga.proto) `TupleKey.object`; `openfga/openfga` [`pkg/tuple/tuple.go`](https://github.com/openfga/openfga/blob/v1.22.0/pkg/tuple/tuple.go) `IsValidObject`; [`internal/validation/validation.go`](https://github.com/openfga/openfga/blob/v1.22.0/internal/validation/validation.go) `ValidateObject` |
| `ValidSubjectCondition(ISubject)` | the subject as written is at most 512 bytes in UTF-8, and: an object (`type:id`) follows the object rules above; a subject set (`type:id#relation`) has a type with no `:`, `#`, space or control character, and an id and a relation that are not empty and have no `:`, `#`, `*`, space or control character; a typed wildcard (`type:*`) has a type that is not empty and has no `:`, `#`, space or control character | `openfga/api` `openfga.proto` `TupleKey.user`; `openfga/openfga` `pkg/tuple/tuple.go` `IsValidObject`, `IsValidUserset`, `IsTypedWildcard`; `internal/validation/validation.go` `ValidateUser` |

### Model lookups

| Condition | Holds when | OpenFGA source |
|---|---|---|
| `KnownTypeCondition(IAuthorizationModel, IString type)` | the model defines the type | `openfga/openfga` `pkg/typesystem/typesystem.go` `GetTypeDefinition` |
| `RelationOfTypeCondition(IAuthorizationModel, IString type, IString relation)` | the model defines the type, and the type defines the relation | `openfga/openfga` `pkg/typesystem/typesystem.go` `GetRelation` |
| `KnownConditionCondition(IAuthorizationModel, IString condition)` | the model defines the condition | `openfga/openfga` `pkg/typesystem/typesystem.go` `validateTypeRestrictions`; `internal/validation/validation.go` `validateCondition` |

### Model consistency

| Condition | Holds when | OpenFGA source |
|---|---|---|
| `SupportedSchemaVersionCondition(IString)` | the schema version is `1.1` or `1.2` | `openfga/openfga` `pkg/typesystem/typesystem.go` `IsSchemaVersionSupported` |
| `UniqueTypeNamesCondition(IAuthorizationModel)` | no two types share a name | `openfga/openfga` `pkg/typesystem/typesystem.go` `containsDuplicateType` |
| `UniqueRelationNamesCondition(ITypeDefinition)` | no two relations of the type share a name | `openfga/api` `authzmodel.proto` `TypeDefinition.relations` (a map keyed by name) |
| `UniqueConditionNamesCondition(IAuthorizationModel)` | no two conditions share a name | `openfga/api` `authzmodel.proto` `AuthorizationModel.conditions` (a map keyed by name); `openfga/openfga` `pkg/typesystem/typesystem.go` `validateConditions` |
| `ValidSubjectFormCondition(IAuthorizationModel, ISubjectForm)` | a type or wildcard form names a defined type; a subject-set form names a defined type and one of its relations; a conditional form conditions a valid, non-conditional form and names a defined condition | `openfga/openfga` `pkg/typesystem/typesystem.go` `validateTypeRestrictions` |
| `ValidTypeRestrictionsCondition(IAuthorizationModel, IRelationDefinition)` | a relation whose rewrite contains a direct node has at least one form, and every form is valid | `openfga/openfga` `pkg/typesystem/typesystem.go` `validateTypeRestrictions` |
| `ResolvableRewriteCondition(IAuthorizationModel, ITypeDefinition, IRelationDefinition)` | a computed relation names another relation of the same type; a relation-from names a tupleset relation of the same type that is exactly a direct node and is assignable only from plain types (conditional or not), at least one of which defines the looked-up relation; union, intersection and difference operands resolve, recursively | `openfga/openfga` `pkg/typesystem/typesystem.go` `isUsersetRewriteValid`, `validateTypeRestrictions` |
| `ValidConditionParameterTypeCondition(IConditionParameterType)` | the type is `any`, `bool`, `string`, `int`, `uint`, `double`, `duration`, `timestamp` or `ipaddress` without generic types, or `map` or `list` with exactly one valid generic type | `openfga/api` `authzmodel.proto` `ConditionParamTypeRef`; `openfga/openfga` [`internal/condition/types/encoding.go`](https://github.com/openfga/openfga/blob/v1.22.0/internal/condition/types/encoding.go) `DecodeParameterType` |
| `ValidConditionParametersCondition(ICondition)` | at most 25 parameters, with unique names of 1 to 50 code points with no `:`, `#`, `@` or `\s`, and valid types | `openfga/api` `authzmodel.proto` `Condition.parameters`; `openfga/openfga` `internal/condition/types/encoding.go` `DecodeParameterType` |
| `ValidAuthorizationModelCondition(IAuthorizationModel)` | a supported schema version; at least one type; unique, valid type names; per type, unique, valid relation names, and per relation a resolvable rewrite and valid type restrictions; at most 25 conditions with unique, valid names, expressions of at most 512 bytes and valid parameters | `openfga/openfga` `pkg/typesystem/typesystem.go` `NewAndValidate`; `openfga/api` [`openfga/v1/openfga_service.proto`](https://github.com/openfga/api/blob/c0650ce2169bcb39a72d73841fb531c200e32248/openfga/v1/openfga_service.proto) `WriteAuthorizationModelRequest`; `authzmodel.proto` `Condition.expression` |

### Tuple conformance

| Condition | Holds when | OpenFGA source |
|---|---|---|
| `AssignableTupleCondition(IAuthorizationModel, ITuple)` | the object, relation and subject are syntactically valid; the model defines the object's type and the relation on it, the subject's type and a subject set's relation; a tupleset relation is a direct node and takes only object subjects; a form of the relation admits the subject (a type form an object of its type, a subject-set form a subject set of the same type and relation, a wildcard form a typed wildcard of its type); a tuple without a condition needs a form without one, and a conditional tuple needs a conditional form with the same condition name; a conditional tuple's condition name is 2 to 256 code points with no space or control character, the condition is defined, and its context is a JSON object whose keys are all parameters of the condition | `openfga/openfga` `internal/validation/validation.go` `ValidateTupleForWrite`, `validateTuplesetRestrictions`, `validateTypeRestrictions`, `validateCondition`, `restrictionFacetMatches`; `pkg/tuple/tuple.go` `IsValidRelation`; `openfga/api` `openfga.proto` `TupleKey.relation`, `RelationshipCondition.name` |

## Not checked

These OpenFGA rules are not checked, so a condition can hold for input that OpenFGA rejects:

- **Entrypoints and cycles.** `NewAndValidate` rejects relations that no tuple can ever satisfy (`hasEntrypoints`, `ErrNoEntrypoints`) and relations that resolve through a cycle (`HasCycle`, `ErrCycle`). Neither is checked.
- **CEL expressions.** OpenFGA compiles each condition's expression (`EvaluableCondition.Compile`), so it must parse, type-check to `bool`, and use parameter names that are valid CEL identifiers. Only the expression's 512-byte limit is checked.
- **Context values.** OpenFGA converts each context value to its parameter's type (`CastContextToTypedParameters`) and rejects control characters in context keys and string values (`ValidateStruct`). Only the context keys are checked.
- **Server limits.** The maximum number of types per model (`MaxTypesPerAuthorizationModel`) and the maximum model size in bytes are server configuration, not part of the model.
- **Inline conditions.** The experimental `$expression` condition (OpenFGA 1.21.0, "Dynamic Conditions") is not recognized. A form or tuple that names it needs a model condition of that name, as the abstractions' contract requires.
- **Schema 1.0 and defaults.** A model must state `1.1` or `1.2`. `WriteAuthorizationModel` fills in `1.1` for an empty schema version; this package treats an empty version as unsupported.
- **Lints of the DSL tooling.** `openfga/language` also reports duplicate type restrictions (`[user, user]`), repeated rewrite operands (`a or a`) and unused conditions, and its identifier rules forbid `*` in type and relation names. The OpenFGA server does not enforce these, so neither does this package.
- **Store state.** Whether a tuple already exists, and limits on writes per request, depend on the store, not on the model.
- **Malformed UTF-16.** OpenFGA works on valid UTF-8. A .NET string with an unpaired surrogate has no UTF-8 form, and its code points are counted as replacement characters.

## Design Principles

- **IBool results** — every condition is an `IBool`, keeping results composable within the Pure type system.
- **Lazy evaluation** — conditions are evaluated only when `.BoolValue` is accessed.
- **False for non-conforming input** — an invalid name, an inconsistent model or a non-conforming tuple evaluates to `false`. The one exception: an implementation of `IRewrite`, `ISubject` or `ISubjectForm` that is none of the known variants throws `ArgumentException`.
- **AOT-compatible** — no reflection; fully compatible with Native AOT compilation.

## Dependencies

- [`Pure.Authorization.OpenFGA.Model.Abstractions`](https://github.com/kudima03/Pure.Authorization.OpenFGA.Model.Abstractions) 0.1.0-preview.0.1.0 — OpenFGA authorization model domain interfaces
- [`Pure.Primitives.Bool.Operations`](https://github.com/kudima03/Pure.Primitives.Bool.Operations) 0.5.1 — `And`, `Or`, `Not`
- [`Pure.Primitives.String.Operations`](https://github.com/kudima03/Pure.Primitives.String.Operations) 1.7.0 — string equality and character conditions
- [`Pure.Primitives.Number.Operations`](https://github.com/kudima03/Pure.Primitives.Number.Operations) 1.5.0 — length comparisons
- [`Pure.Primitives.Char.Operations`](https://github.com/kudima03/Pure.Primitives.Char.Operations) 0.4.0 — control character detection
