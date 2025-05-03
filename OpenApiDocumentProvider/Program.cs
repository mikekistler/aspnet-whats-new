using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Writers;

const string documentName = "v1";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(documentName);

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
.WithName("GetWeatherForecast");

if (args.Any(arg => arg.Equals("--generate-openapi")))
{
    var documentProvider = app.Services.GetRequiredKeyedService<IOpenApiDocumentProvider>(documentName);

    var document = await documentProvider.GetOpenApiDocumentAsync();

    // Serialize the OpenAPI document to disk.
    var path = $"{documentName}.json";
    using var fileStream = new FileStream(path, FileMode.Create);
    using var writer = new StreamWriter(fileStream);
    var jsonWriter = new OpenApiJsonWriter(writer);
    await document.SerializeAsync(jsonWriter, Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1);
    await writer.FlushAsync();

    writer.Dispose();
    fileStream.Dispose();

    Console.WriteLine($"OpenAPI document generated at {path}");
    return;
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
