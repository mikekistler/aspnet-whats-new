# aspnet-whats-new

What's new in ASP.NET Core preview 4.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/35217 -->

- New `WithOpenApiOperationTransformer` extension method for `IEndpointRouteBuilder` to add OpenAPI operation metadata to endpoints.
  - This was actually added in preview 3 but the docs were not updated.
- Support for generating OpenApiSchemas in transformers
- Support for JSON Patch with System.Text.Json
- Various bug fixes to the XML comment generator -- it should throw fewer build errors now.
It should also work with the Identity API XML comments but I haven't verified that.
- Some bug fixes on the validations generator.
- Support for IOpenApiDocumentProvider in the DI container. Makes it so that you can generate an OpenAPIDocument in-memory from anywhere.

## Enhanced OpenAPI Support with Operation Transformers

<!-- https://github.com/dotnet/aspnetcore/pull/60566 -->

The new `AddOpenApiOperationTransformer` API makes it easier to customize OpenAPI documentation for your ASP.NET Core endpoints. This API allows you to register custom operation transformers, which modify OpenAPI operation definitions programmatically.
This feature reduces the need for manual intervention or external tools, streamlining the API documentation process.

### Key Features:
- **Flexible Transformations**: Use custom or predefined logic to modify OpenAPI operations at runtime.
- **Support for Multiple Transformers**: Chain multiple transformers to apply different transformations sequentially.

### Example: Custom transformer

Here’s how you can use the `AddOpenApiOperationTransformer` extension method with a custom transformer:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!")
    .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Description = "This endpoint returns a greeting message.";
        return Task.CompletedTask;
    });

app.Run();
```

### Example: Predefined and chained transformers

You can also create predefined transformers that you can use on multiple endpoints. These are defined as extension methods on `RouteHandlerBuilder`, and return a `RouteHandlerBuilder` so they can be chained with other methods like `WithName`, `WithTags`, etc.
Some example use cases are a transformer to add a description for a specific response code, or a transformer to add a response header.

```csharp
public static class ExtensionMethods
{
    public static RouteHandlerBuilder WithResponseDescription(this RouteHandlerBuilder builder, int statusCode, string description)
    {
        builder.AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
        {
            var response = operation.Responses?.TryGetValue(statusCode.ToString(), out var r) == true ? r : null;
            // The following line uses the new "null conditional assignment" feature of C# 14
            response?.Description = description;
            return Task.CompletedTask;
        });
        return builder;
    }

    public static RouteHandlerBuilder WithLocationHeader(this RouteHandlerBuilder builder)
    {
        builder.AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
        {
            var createdResponse = operation?.Responses?.TryGetValue("201", out var r) == true ? r : null;
            // The following line uses the new "null conditional assignment" feature of C# 14
            createdResponse?.Headers["Location"] = new OpenApiHeader
            {
                Description = "Location of the created resource.",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "uri"
                }
            };
            return Task.CompletedTask;
        });
        return builder;
    }
}
```

Here's how you can use the above transformers in your application:

```csharp
app.MapPost("/todos", (Todo todo) =>
        TypedResults.Created($"/todos/{todo.Id}", todo))
    .WithName("CreateTodo")
    .WithResponseDescription(201, "The todo was created successfully.")
    .WithLocationHeader();
```

and the resulting OpenAPI document will look like this:

<!-- In the docs, highlight the response description and response header -->
```json
  "paths": {
    "/todos": {
      "post": {
        "operationId": "CreateTodo",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/Todo"
              }
            }
          },
          "required": true
        },
        "responses": {
          "201": {
            "description": "The todo was created successfully.",
            "headers": {
              "Location": {
                "description": "Location of the created resource.",
                "required": true,
                "schema": {
                  "type": "string",
                  "format": "uri"
                }
              }
            },
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/Todo"
                }
              }
            }
          }
        }
      }
    }
```
