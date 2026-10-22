# Changelog

All notable changes to `Pure.Authorization.OpenFGA.Model.Conditions` are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Identifier syntax conditions following OpenFGA's validation rules: `ValidTypeNameCondition`, `ValidRelationNameCondition`, `ValidConditionNameCondition`, `ValidObjectCondition` and `ValidSubjectCondition`.
- Model lookup conditions: `KnownTypeCondition`, `RelationOfTypeCondition` and `KnownConditionCondition`.
- Model consistency conditions: `SupportedSchemaVersionCondition`, `UniqueTypeNamesCondition`, `UniqueRelationNamesCondition`, `UniqueConditionNamesCondition`, `ValidSubjectFormCondition`, `ValidTypeRestrictionsCondition`, `ResolvableRewriteCondition`, `ValidConditionParameterTypeCondition`, `ValidConditionParametersCondition` and the aggregate `ValidAuthorizationModelCondition`.
- Tuple conformance condition `AssignableTupleCondition`: type restrictions, tupleset restrictions and condition matching as OpenFGA applies them on write.
- Package validation is enabled, with no baseline until the first release.
