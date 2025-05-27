# aspnet-whats-new

What's new in ASP.NET Core preview 5.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/35473 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A10.0-preview5+is%3Amerged+label%3Aarea-minimal%2Carea-mvc -->

Here's a summary of what's new in ASP.NET Core in this preview release:

- [Validation in Minimal APIs](#validation-in-minimal-apis)
- [Support for generating OpenAPI 3.1](#support-for-generating-openapi-31)
- [OpenAPI metadata from XML doc comments](#openapi-metadata-from-xml-doc-comments)

## Validation in Minimal APIs

<!-- https://github.com/dotnet/aspnetcore/pull/61952 Fix sanitization of type names in validations generator -->
<!-- https://github.com/dotnet/aspnetcore/pull/61927 Fix trim annotations on generated validation code -->
<!-- https://github.com/dotnet/aspnetcore/pull/61895 Exempt parameters resolved from DI from validation -->
<!-- https://github.com/dotnet/aspnetcore/pull/61862 Mark validations info related types are experimental -->
<!-- https://github.com/dotnet/aspnetcore/pull/61778 Make IValidatableObject handling more resilient for MemberNames -->
<!-- https://github.com/dotnet/aspnetcore/pull/61766 Fix handling of nullable types in validations generator -->
<!-- https://github.com/dotnet/aspnetcore/pull/61728 Fix handling of parsable types in validations generator -->

A number of small improvements and fixes have been made to the validation generator for Minimal APIs that was introduced in preview 4.
In addition, all the validation-related types have been marked as experimental, but validation using existing data annotations and the `AddValidation()` method is still considered stable.

## Support for generating OpenAPI 3.1

<!-- https://github.com/dotnet/aspnetcore/pull/61928 -->

The OpenAPI.NET library used in ASP.NET Core OpenAPI document generation has been upgraded to [v2.0.0-preview18](https://github.com/microsoft/OpenAPI.NET/releases/tag/v2.0.0-preview.18).

## OpenAPI metadata from XML doc comments

<!-- https://github.com/dotnet/aspnetcore/pull/61920 -->

Support for generating OpenAPI metadata from XML doc comments has been extended to extract metadata for operation responses
from `<returns>` and `<response>` XML tags.

<!--
## JSON Patch with System.Text.Json

## Generate correct unauthorized responses for APIs in Web apps
-->
