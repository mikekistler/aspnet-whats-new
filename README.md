# aspnet-whats-new

What's new in ASP.NET Core for .NET 11 preview 1.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/36655 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A11.0-preview1+is%3Amerged+label%3Aarea-minimal%2Carea-mvc -->

Here's a summary of what's new in ASP.NET Core in this preview release:

- Add support for FileContentResult in OpenAPI schemas

## Support for binary file response in OpenAPI schemas

<!-- https://github.com/dotnet/aspnetcore/pull/63504 -->

New in ASP.NET Core 11 Preview 1 is support for generating OpenAPI descriptions for operations that return binary file responses. The new support maps the [FileContentResult] result type to an OpenAPI schema with `type: string` and `format: binary`. This support is available for both Minimal APIs and controller-based apps.

The following example shows a Minimal API endpoint that returns binary content and uses the [Produces\<T\>] extension method with `T` of [FileContentResult] to specify the response type and content type.

```csharp
app.MapPost("/filecontentresult", () =>
{
    var content = "This endpoint returns a FileContentResult!"u8.ToArray();
    return TypedResults.File(content);
})
.Produces<FileContentResult>(contentType: MediaTypeNames.Application.Octet);
```

In the generated OpenAPI document, the endpoint response is described like this:

```yaml
  responses:
    '200':
      description: OK
      content:
        application/octet-stream:
          schema:
            $ref: '#/components/schemas/FileContentResult'
```

with `FileContentResult` defined in `components/schemas` as:

```yaml
components:
  schemas:
    FileContentResult:
      type: string
      format: binary
```

In a controller-based app, use the [ProducesResponseType\<T\>] attribute with `T` of [FileContentResult] to specify the response type and content type as shown in this example:

```csharp
[HttpPost("filecontentresult")]
[ProducesResponseType<FileContentResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet)]
public IActionResult PostFileContentResult()
{
    var content = "This endpoint returns a FileContentResult!"u8.ToArray();
    return new FileContentResult(content, MediaTypeNames.Application.Octet);
}
```

This operation will have the same OpenAPI description as shown above.

[FileContentResult]: https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.filecontentresult
[Produces\<T\>]: https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.openapiroutehandlerbuilderextensions.produces?view=aspnetcore-10.0#microsoft-aspnetcore-http-openapiroutehandlerbuilderextensions-produces-1(microsoft-aspnetcore-builder-routehandlerbuilder-system-int32-system-string-system-string())
[ProducesResponseType\<T\>]: https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.producesresponsetypeattribute-1?view=aspnetcore-10.0