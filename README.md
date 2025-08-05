# aspnet-whats-new

What's new in ASP.NET Core for .NET 10 Preview 7.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/35824 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A10.0-preview7+is%3Amerged+label%3Aarea-minimal%2Carea-mvc -->

<!-- https://github.com/dotnet/core/tree/main/release-notes -->

<!--
Summarize the changes in ths PR from a user perspective. Format your response as markdown.
Create a summary of the changes/improvements in this PR that a user would care about. Use markdown formatting.
Describe the user-facing changes/improvements in this PR. Use markdown formatting.
-->

Here's a summary of what's new in ASP.NET Core in this preview release:

- [Upgrade Microsoft.OpenApi to 2.0.0](#upgrade-microsoftopenapi-to-200)
- [Enhance validation for classes and records](#enhance-validation-for-classes-and-records)
- [Fix ProducesResponseType Description for Minimal APIs](#fix-producesresponsetype-description-for-minimal-apis)
- [Correct metadata type for formdata enum parameters](#correct-metadata-type-for-formdata-enum-parameters)
- [Unify handling of documentation IDs in OpenAPI XML comment generator](#unify-handling-of-documentation-ids-in-openapi-xml-comment-generator)

## Upgrade Microsoft.OpenApi to 2.0.0

<!-- https://github.com/dotnet/aspnetcore/pull/62719 -->

The OpenAPI.NET library used in ASP.NET Core OpenAPI document generation has been upgraded to v2.0.0 (GA).
With the update to the GA version of this package, no further breaking changes are expected in the OpenAPI document generation.

## Enhance validation for classes and records

<!-- https://github.com/dotnet/aspnetcore/pull/62633 -->

Users can now use validation attributes on both classes and records, with consistent code generation and validation behavior. This enhances flexibility when designing models using records in ASP.NET Core applications.

** Community contribution: Thanks to @marcominerva **

## Fix ProducesResponseType Description for Minimal APIs

<!-- https://github.com/dotnet/aspnetcore/pull/62695 -->

The Description property for the `ProducesResponseType` attribute is now correctly set in Minimal APIs even when the attribute type and the inferred return type are not an exact match.

** Community contribution: Thanks to @sander1095 **

## Correct metadata type for formdata enum parameters

<!-- https://github.com/dotnet/aspnetcore/pull/61399 -->

The metadata type for formdata enum parameters in MVC controller actions has been updated to use the actual enum type instead of string.

** Community contribution: Thanks to @ascott18 **

## Unify handling of documentation IDs in OpenAPI XML comment generator

<!-- https://github.com/dotnet/aspnetcore/pull/62692 -->

XML documentation comments from referenced assemblies are now correctly merged if their documentation IDs included return type suffixes.
As a result, all valid XML comments are now reliably included in generated OpenAPI documentation, improving doc accuracy and completeness for APIs using referenced assemblies.
