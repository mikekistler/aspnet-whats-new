# aspnet-whats-new

What's new in ASP.NET Core for .NET 11 preview 1.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/36655 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A11.0-preview1+is%3Amerged+label%3Aarea-minimal%2Carea-mvc -->

Here's a summary of what's new in ASP.NET Core in this preview release:

- Add support for FileContentResult in OpenAPI schemas

## Add support for FileContentResult in OpenAPI schemas

<!-- https://github.com/dotnet/aspnetcore/pull/63504 -->

The framework now supports generating OpenAPI schemas for `FileContentResult` in ASP.NET Core Web APIs.
The schema for `FileContentResult` is generated as `type: string, format: binary`.
