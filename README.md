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
- [Include property descriptions as siblings of $ref in OpenAPI schema](#include-property-descriptions-as-siblings-of-ref-in-openapi-schema)
- [Exclude unknown HTTP methods from OpenAPI](#exclude-unknown-http-methods-from-openapi)
- [Apply the correct media type for JSON Patch](#apply-the-correct-media-type-for-json-patch)

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

## Include property descriptions as siblings of $ref in OpenAPI schema

Prior to .NET 10, ASP.NET Core discarded descriptions on properties that were defined with `$ref` in the generated OpenAPI document.
This was necessary because OpenAPI v3.0 did not allow sibling properties alongside `$ref` in schema definitions. But this restriction has been relaxed in OpenAPI 3.1, allowing descriptions to be included alongside `$ref`. Support was added in RC1 to include property descriptions as siblings of `$ref` in the generated OpenAPI schema.

This was a community contribution. Thanks @desjoerd!

## Exclude unknown HTTP methods from OpenAPI

<!-- https://github.com/dotnet/aspnetcore/pull/63034, https://github.com/dotnet/aspnetcore/pull/63092 -->

OpenAPI schema generation now excludes unknown HTTP methods from the generated OpenAPI document.
In particular, query methods, which are standard HTTP methods but not recognized by OpenAPI,
are now gracefully excluded from the generated OpenAPI document.

This was a community contribution. Thanks @martincostello!

## Improve the description of JSON Patch request bodies

<!-- https://github.com/dotnet/aspnetcore/pull/62988 -->
<!-- https://github.com/dotnet/aspnetcore/pull/63052 -->

The OpenAPI schema generation for JSON Patch operations now correctly applies the `application/json-patch+json` media type to request bodies that use JSON Patch. This ensures that the generated OpenAPI document accurately reflects the expected media type for JSON Patch operations.
In addition, the JSON Patch request body has a detailed schema that describes the structure of the JSON Patch document, including the operations that can be performed.

This was a community contribution. Thanks @martincostello!

## Use invariant culture for OpenAPI document generation

<!-- https://github.com/dotnet/aspnetcore/pull/62193 -->

OpenAPI document generation now uses invariant culture for formatting numbers and dates in the generated OpenAPI document. This ensures that the generated document is consistent and does not vary based on the server's culture settings.

This was a community contribution. Thanks @martincostello!
