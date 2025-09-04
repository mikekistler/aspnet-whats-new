# aspnet-whats-new

What's new in ASP.NET Core for .NET 10 RC1.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/35824 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A10.0-rc1+is%3Amerged+label%3Aarea-minimal%2Carea-mvc -->

<!-- https://github.com/dotnet/core/tree/main/release-notes -->

<!--
Summarize the changes in ths PR from a user perspective. Format your response as markdown.
Create a summary of the changes/improvements in this PR that a user would care about. Use markdown formatting.
Describe the user-facing changes/improvements in this PR. Use markdown formatting.
-->

Here's a summary of what's new in ASP.NET Core in this RC release:

- [Model nullable types using oneOf in OpenAPI schema](#model-nullable-types-using-oneof-in-openapi-schema)
- [Fixes/improvements to schema reference resolution](#fixesimprovements-to-schema-reference-resolution)

## Model nullable types using oneOf in OpenAPI schema

<!-- https://github.com/dotnet/aspnetcore/pull/63325 -->

OpenAPI schema generation for nullable types was improved by using the `oneOf` pattern instead of the nullable property for complex types and collections. The implementation:

- Uses `oneOf` with `null` and the actual type schema for nullable complex types in request/response schemas
- Implements proper nullability detection for parameters, properties, and return types using reflection and NullabilityInfoContext
- Prunes null types from componentized schemas to avoid duplication

## Fixes/improvements to schema reference resolution

<!-- https://github.com/dotnet/aspnetcore/pull/63256 -->

### User-Facing Changes in PR #63256: "Resolve relative JSON schema references in root schema"

This release improves the handling of JSON schemas for OpenAPI document generation by properly resolving relative JSON schema references (`$ref`) in the root schema document.
