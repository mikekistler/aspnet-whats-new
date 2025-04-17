# aspnet-whats-new

What's new in ASP.NET Core preview 4.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/35217 -->

- New `WithOpenApiOperationTransformer` extension method for `IEndpointRouteBuilder` to add OpenAPI operation metadata to endpoints.
  - This was actually added in preview 3 but the docs were not updated.
- Support for generating OpenApiSchemas in transformers
- Various bug fixes to the XML comment generator -- it should throw fewer build errors now.
It should also work with the Identity API XML comments but I haven't verified that.
- Some bug fixes on the validations generator.
- Support for IOpenApiDocumentProvider in the DI container. Makes it so that you can generate an OpenAPIDocument in-memory from anywhere.

## `WithOpenApiOperationTransformer` extension method

<!-- https://github.com/dotnet/aspnetcore/pull/60566 -->

