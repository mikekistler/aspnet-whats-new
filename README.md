# aspnet-whats-new

What's new in ASP.NET Core for .NET 11 preview 1.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/36655 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A11.0-preview1+is%3Amerged+label%3Aarea-minimal%2Carea-mvc -->

Here's a summary of what's new in ASP.NET Core in this preview release:

- Add support for FileContentResult in OpenAPI schemas

## Support for FileContentResult in OpenAPI schemas

<!-- https://github.com/dotnet/aspnetcore/pull/63504 -->

ASP.NET Core now generates valid response schemas for action methods of controller-based apps that return a [FileContentResult].
The schema for [FileContentResult] is generated as `type: string, format: binary`.
For example, an action method like this:

```csharp
[HttpPost("filecontentresult")]
[ProducesResponseType<FileContentResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet)]
public IActionResult PostFileContentResult()
{
    var content = "This endpoint returns a FileContentResult!"u8.ToArray();
    return new FileContentResult(content, MediaTypeNames.Application.Octet);
}
```
Will be documented in OpenAPI with these response details:

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
  FileContentResult:
    type: string
    format: binary

```yaml
components:
  schemas:
    FileContentResult:
      type: string
      format: binary
```

The equivalent functionality is also available for Minimal APIs with the `Produces<T>` extension method.
In a Minimal API app, the endpoint above can be defined like this:

```csharp
app.MapPost("/filecontentresult", () =>
{
    var content = "This endpoint returns a FileContentResult!"u8.ToArray();
    return TypedResults.Bytes(content);
})
.Produces<byte[]>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet);
```

and will be documented in OpenAPI as follows:

```yaml
  /filecontentresult:
    post:
      tags:
        - FileContentResult
      responses:
        '200':
          description: OK
          content:
            application/octet-stream:
              schema:
                type: string
                format: byte
```

[FileContentResult]: https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.mvc.filecontentresult