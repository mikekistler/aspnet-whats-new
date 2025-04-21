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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.AddOpenApiOperationTransformer((operation, context, token) =>
{
    operation.Responses["200"].Description = "A custom description for the 200 response.";
    operation.Responses["200"].Headers["X-Custom-Header"] = new OpenApiHeader
    {
        Description = "A custom header for the 200 response.",
        Required = false,
        Schema = new OpenApiSchema
        {
            Type = JsonSchemaType.String
        }
    };
    return Task.CompletedTask;
});

app.MapPost("/weatherforecast", (WeatherForecast forecast) =>
{
    return TypedResults.Created($"/weatherforecast/{forecast.Date}", forecast);
})
.WithName("CreateWeatherForecast")
.WithResponseDescription(201, "The forecast was created successfully.")
.WithResponseDescription(418, "I'm a teapot.")
.WithLocationHeader();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Static class for operation transformers
public static class ExtensionMethods
{
    public static RouteHandlerBuilder WithResponseDescription(this RouteHandlerBuilder builder, int statusCode, string description)
    {
        builder.AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
        {
            // operation.Responses?.TryGetValue(statusCode.ToString(), out var response);
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
