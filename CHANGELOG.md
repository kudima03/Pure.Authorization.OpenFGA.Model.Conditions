# Changelog

All notable changes to `Pure.Authorization.OpenFGA.Model.Conditions` are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0-preview.0.2.0] — 2026-11-11

### Added

- `net11.0` target framework.

### Changed

- Updated `Pure.Authorization.OpenFGA.Model.Abstractions`, `Pure.Primitives.Bool.Operations`, `Pure.Primitives.Char.Operations`, `Pure.Primitives.Number.Operations` and `Pure.Primitives.String.Operations` to their .NET 11 releases.

## [0.1.0-preview.0.1.0] — 2026-10-23

### Added

- Identifier syntax conditions following OpenFGA's validation rules: `ValidTypeNameCondition`, `ValidRelationNameCondition`, `ValidConditionNameCondition`, `ValidObjectCondition` and `ValidSubjectCondition`.
- Model lookup conditions: `KnownTypeCondition`, `RelationOfTypeCondition` and `KnownConditionCondition`.
- Model consistency conditions: `SupportedSchemaVersionCondition`, `UniqueTypeNamesCondition`, `UniqueRelationNamesCondition`, `UniqueConditionNamesCondition`, `ValidSubjectFormCondition`, `ValidTypeRestrictionsCondition`, `ResolvableRewriteCondition`, `ValidConditionParameterTypeCondition`, `ValidConditionParametersCondition` and the aggregate `ValidAuthorizationModelCondition`.
- Tuple conformance condition `AssignableTupleCondition`: type restrictions, tupleset restrictions and condition matching as OpenFGA applies them on write.
- Package validation is enabled, with no baseline until the first release.
