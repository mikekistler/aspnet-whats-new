using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/hello", () => "Hello World!")
    .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Responses?["200"].Description = "A cheerful greeting message.";
        return Task.CompletedTask;
    });

app.MapPost("/todos", (Todo todo) =>
        TypedResults.Created($"/todos/{todo.Id}", todo))
    .WithName("CreateTodo")
    .WithResponseDescription(201, "The todo was created successfully.")
    .WithLocationHeader();

app.Run();

record Todo(string Id, string Title, bool IsComplete);

// Static class for operation transformers
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
