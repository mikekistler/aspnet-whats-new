# aspnet-whats-new

What's new in ASP.NET Core preview 4.

<!-- https://github.com/dotnet/AspNetCore.Docs/issues/35217 -->

<!-- https://github.com/dotnet/aspnetcore/pulls?q=is%3Apr+milestone%3A10.0-preview4+is%3Aclosed+label%3Aarea-minimal%2Carea-mvc -->

Here's a summary of what's new in ASP.NET Core in this preview release:

- [Enhanced OpenAPI Support with Operation Transformers](#enhanced-openapi-support-with-operation-transformers)

- [New JsonPatch Implementation with System.Text.Json](#new-jsonpatch-implementation-with-systemtextjson)

- [Support for generating OpenApiSchemas in transformers](#support-for-generating-openapischemas-in-transformers)

- [Improvements to XML comment generator](#improvements-to-xml-comment-generator)

- [Improvements to Minimal APIs Validation](#improvements-to-minimal-apis-validation)

- [Support for IOpenApiDocumentProvider in the DI container](#support-for-iopenapidocumentprovider-in-the-di-container)

- [OpenAPI.NET updated to Preview.17](#openapinet-updated-to-preview17)

## Enhanced OpenAPI Support with Operation Transformers

<!-- https://github.com/dotnet/aspnetcore/pull/60566 -->
<!-- This was actually added in preview 3 but the docs were not updated. -->

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

## New JsonPatch Implementation with System.Text.Json

<!-- https://github.com/dotnet/aspnetcore/pull/61313 -->

JSON Patch is a standard format for describing changes to apply to a JSON document, defined in [RFC 6902].
It represents a sequence of operations (e.g., add, remove, replace, move, copy, test) that can be applied to modify a JSON document.
In web applications, JSON Patch is commonly used in a PATCH operation to perform partial updates of a resource.
Instead of sending the entire resource for an update, clients can send a JSON Patch document containing only the changes.
This reduces payload size and improves efficiency.

[RFC 6902]: https://tools.ietf.org/html/rfc6902

This release introduces a new implementation of `JsonPatch` based on `System.Text.Json` serialization.
This feature aligns with modern .NET practices by leveraging the `System.Text.Json` library, which is optimized for .NET.
This feature provides improved performance and reduced memory usage compared to the legacy `Newtonsoft.Json`-based implementation.

The following benchmarks compare the performance of the new `System.Text.Json` implementation with the legacy `Newtonsoft.Json` implementation:

| Scenario                   | Implementation         | Mean       | Allocated Memory |
|----------------------------|------------------------|------------|------------------|
| **Application Benchmarks** | Newtonsoft.JsonPatch   | 271.924 µs | 25 KB            |
|                            | System.Text.JsonPatch  | 1.584 µs   | 3 KB             |
| **Deserialization Benchmarks** | Newtonsoft.JsonPatch | 19.261 µs  | 43 KB            |
|                            | System.Text.JsonPatch  | 7.917 µs   | 7 KB             |

These benchmarks highlight significant performance gains and reduced memory usage with the new implementation.

Notes:
- The new implementation is not a drop-in replacement for the legacy implementation. In particular:
  - The new implementation doesn't support dynamic types (like `ExpandoObject`).
  - The new implementation uses the declared type of the target object to determine the properties to patch, and not the runtime type as was the case with the   legacy implementation.
- The JSON Patch standard has inherent security risks. Since these risks are inherent to the JSON Patch standard, the new implementation does not attempt to mitigate them. It is the responsibility of the developer to ensure that the JSON Patch document is safe to apply to the target object. See the [Mitigating Security Risks](#mitigating-security-risks) section for more information.

### Usage

To enable JSON Patch support with `System.Text.Json`, install the `Microsoft.AspNetCore.JsonPatch.SystemTextJson` NuGet package.

```sh
dotnet add package Microsoft.AspNetCore.JsonPatch.SystemTextJson --prerelease
```

This package provides a `JsonPatchDocument<T>` class to represent a JSON Patch document for objects of type `T`
and custom logic for serializing and deserializing JSON Patch documents using `System.Text.Json`.
The key method of the `JsonPatchDocument<T>` class is `ApplyTo`, which applies the patch operations to a target object of type `T`.

The following examples demonstrate how to use the `ApplyTo` method to apply a JSON Patch document to an object.

### Example: Applying a JsonPatchDocument

The following example demonstrates:
1. The "add", "replace", and "remove" operations.
2. Operations on nested properties.
3. Adding a new item to an array.
4. Using a JSON String Enum Converter in a JSON patch document.

```csharp
// Original object
var person = new Person {
    FirstName = "John",
    LastName = "Doe",
    Email = "johndoe@gmail.com",
    PhoneNumbers = [new() {Number = "123-456-7890", Type = PhoneNumberType.Mobile}],
    Address = new Address
    {
        Street = "123 Main St",
        City = "Anytown",
        State = "TX"
    }
};

// Raw JSON patch document
string jsonPatch = """
[
    { "op": "replace", "path": "/FirstName", "value": "Jane" },
    { "op": "remove", "path": "/Email"},
    { "op": "add", "path": "/Address/ZipCode", "value": "90210" },
    { "op": "add", "path": "/PhoneNumbers/-", "value": { "Number": "987-654-3210", "Type": "Work" } }
]
""";

// Deserialize the JSON patch document
var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<Person>>(jsonPatch);

// Apply the JSON patch document
patchDoc!.ApplyTo(person);

// Output updated object
Console.WriteLine(JsonSerializer.Serialize(person, serializerOptions));

// Output:
// {
//   "firstName": "Jane",
//   "lastName": "Doe",
//   "address": {
//     "street": "123 Main St",
//     "city": "Anytown",
//     "state": "TX",
//     "zipCode": "90210"
//   },
//   "phoneNumbers": [
//     {
//       "number": "123-456-7890",
//       "type": "Mobile"
//     },
//     {
//       "number": "987-654-3210",
//       "type": "Work"
//     }
//   ]
// }
```

### Example: Applying a JsonPatchDocument with error handling

There are a variety of errors that can occur when applying a JSON Patch document.
For example, the target object may not have the specified property, or the value specified may be incompatible with the property type.
JSON Patch also supports the `test` operation, which checks if a specified value is equal to the target property,
and if not, this is considered an error.

The following example demonstrates how to handle these errors gracefully.

> Important: The object passed to the `ApplyTo` method is modified in place. It is the caller's
> responsiblity to discard these changes if any operation fails.

```csharp
// Original object
var person = new Person {
    FirstName = "John",
    LastName = "Doe",
    Email = "johndoe@gmail.com"
};

// Raw JSON patch document
string jsonPatch = """
[
    { "op": "replace", "path": "/Email", "value": "janedoe@gmail.com"},
    { "op": "test", "path": "/FirstName", "value": "Jane" },
    { "op": "replace", "path": "/LastName", "value": "Smith" }
]
""";

// Deserialize the JSON patch document
var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<Person>>(jsonPatch);

// Apply the JSON patch document, catching any errors
Dictionary<string, string[]>? errors = null;
patchDoc!.ApplyTo(person, jsonPatchError =>
    {
        errors ??= new ();
        var key = jsonPatchError.AffectedObject.GetType().Name;
        if (!errors.ContainsKey(key))
        {
            errors.Add(key, new string[] { });
        }
        errors[key] = errors[key].Append(jsonPatchError.ErrorMessage).ToArray();
    });
if (errors != null)
{
    // Print the errors
    foreach (var error in errors)
    {
        Console.WriteLine($"Error in {error.Key}: {string.Join(", ", error.Value)}");
    }
}

// Output updated object
Console.WriteLine(JsonSerializer.Serialize(person, serializerOptions));

// Output:
// Error in Person: The current value 'John' at path 'FirstName' is not equal to the test value 'Jane'.
// {
//   "firstName": "John",
//   "lastName": "Smith",              <<< Modified!
//   "email": "janedoe@gmail.com",     <<< Modified!
//   "phoneNumbers": []
// }
```

### Mitigating Security Risks

When using the .JsonPatch[.SystemTextJson] package, it is critical to understand and mitigate potential security risks.
Below are the identified threats along with their corresponding mitigations to ensure secure usage of the package.

> [!IMPORTANT]
> This is not an exhaustive list of threats. Application developers must conduct their own threat model reviews to determine an application-specific comprehensive list and come up with appropriate mitigations as needed. For example, applications which expose collections to patch operations should consider the potential for algorithmic complexity attacks if those operations insert or remove elements at the beginning of the collection.

By running comprehensive threat models for their own applications and addressing identified threats while following the recommended mitigations below, consumers of these packages can safely integrate JSON Patch functionality into their applications while minimizing security risks.

#### Denial of Service (DoS) via Memory Amplification

**Scenario**: A malicious client submits a `copy` operation that duplicates large object graphs multiple times, leading to excessive memory consumption.
**Impact**: Potential Out-Of-Memory (OOM) conditions, causing service disruptions.
**Mitigation**:
- Validate incoming JSON Patch documents for size and structure before applying the document before calling `ApplyTo` method.
- The validation will need to be application specific, of course, but an example validation can look something like the following:

```csharp
public void Validate(JsonPatchDocument<T> patch)
{
    // This is just an example. It's up to the developer to make sure that this case is handled properly,
    // based on the application needs.
    if (patch.Operations.Where(op=>op.OperationType == OperationType.Copy).Count() > MaxCopyOperationsCount)
    {
        throw new InvalidOperationException();
    }
}
```

#### Business Logic Subversion

**Scenario**: Patch operations can manipulate fields with implicit invariants (e.g., internal flags, IDs, or computed fields), violating business constraints.
**Impact**: Data integrity issues and unintended application behavior.
**Mitigation**:
- Use POCO objects with explicitly defined properties that are safe to modify.
- Avoid exposing sensitive or security-critical properties in the target object.
- If no POCO object is used, validate the patched object after applying operations to ensure business rules and invariants are not violated.

#### Authentication and Authorization

**Scenario**: Unauthenticated or unauthorized clients send malicious JSON Patch requests.
**Impact**: Unauthorized access to modify sensitive data or disrupt application behavior.
**Mitigation**:
- Protect endpoints accepting JSON Patch requests with proper authentication and authorization mechanisms.
- Restrict access to trusted clients or users with appropriate permissions.

## Support for generating OpenApiSchemas in transformers
<!-- https://github.com/dotnet/aspnetcore/pull/61050 -->

Developers can now generate a schema for a C# type, using the same logic as ASP.NET Core OpenAPI document generation,
and add it to the OpenAPI document. The schema can then be referenced from elsewhere in the OpenAPI document.

The context passed to document, operation, and schema transformers now has a `GetOrCreateSchemaAsync` method that can be used to generate a schema for a type.
This method also has an optional `ApiParameterDescription` parameter to specify additional metadata for the generated schema.

To support adding the schema to the OpenAPI document, a `Document` property has been added to the Operation and Schema transformer contexts. This allows any transformer to add a schema to the OpenAPI document using the document's `AddComponent` method.

### Example

To use this feature in an document, operation, or schema transformer, create the schema using the `GetOrCreateSchemaAsync` method provided in the context
and add it to the OpenAPI document using the document's `AddComponent` method.

<!-- In the docs, highlight the lines with the call to "GetOrCreateSchemaAsync" and "AddComponent" -->
```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer(async (operation, context, cancellationToken) =>
    {
        // Generate schema for error responses
        var errorSchema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetails), null, cancellationToken);
        context.Document?.AddComponent("Error", errorSchema);

        operation.Responses ??= new OpenApiResponses();
        // Add a "4XX" response to the operation with the newly created schema
        operation.Responses["4XX"] = new OpenApiResponse
        {
            Description = "Bad Request",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/problem+json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchemaReference("Error", context.Document)
                }
            }
        };
    });
});
```

## Improvements to XML comment generator

<!-- https://github.com/dotnet/aspnetcore/pull/61145 -->

<!-- it should throw fewer build errors now.
- It should also work with the Identity API XML comments but I haven't verified that.
  - Maybe update docs about failure mode. -->

The XML comment generator has been enhanced to better handle complex types.
In conjunction, the generator now gracefully bypasses processing for complex types that previously
caused build errors. Taken together, these changes improve the robustness of XML comment generation
but change the failure mode for certain scenarios from build errors to missing metadata.

In addition, XML doc comment processing can now be configured to access XML comments in other assemblies.
This is useful for generating documentation for types that are defined outside the current assembly,
such as the `ProblemDetails` type in the `Microsoft.AspNetCore.Http` namespace.
This configuration is done with directives in the project build file.
The following example shows how to configure the XML comment generator to access XML comments
for types in the `Microsoft.AspNetCore.Http` assembly, which includes the `ProblemDetails` class.

```xml
  <Target Name="AddOpenApiDependencies" AfterTargets="ResolveReferences">
    <ItemGroup>
      <!-- Include XML documentation from Microsoft.AspNetCore.Http.Abstractions to get metadata for ProblemDetails -->
      <AdditionalFiles
            Include="@(ReferencePath->'%(RootDir)%(Directory)%(Filename).xml')"
            Condition="'%(ReferencePath.Filename)' == 'Microsoft.AspNetCore.Http.Abstractions'"
            KeepMetadata="Identity;HintPath" />
    </ItemGroup>
  </Target>
```

We expect to include XML comments from a selected set of assemblies in the shared framework in future previews,
to avoid the need for this configuration in most cases.

## Improvements to Minimal APIs Validation

<!-- https://github.com/dotnet/aspnetcore/pull/61193 -->
<!-- https://github.com/dotnet/aspnetcore/pull/61402 -->

Now supports validation on record types.

## Support for IOpenApiDocumentProvider in the DI container.

<!-- https://github.com/dotnet/aspnetcore/pull/61463 -->

Makes it so that you can generate an OpenAPIDocument in-memory from anywhere.

## OpenAPI.NET updated to Preview.17

<!-- https://github.com/dotnet/aspnetcore/pull/61541 -->

<!-- Preview.3 used OpenAPI.NET v2-preview.11 -->

