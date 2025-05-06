using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;

const string documentName = "v1";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(documentName);

var app = builder.Build();

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

app.MapGet("/openapi/{documentName}.json", async Task<Results<Ok<string>, NotFound<ProblemDetails>>> (string documentName) =>
{
    var documentProvider = app.Services.GetRequiredKeyedService<IOpenApiDocumentProvider>(documentName);
    if (documentProvider == null)
    {
        return TypedResults.NotFound<ProblemDetails>(new (){
            Detail = $"OpenAPI document '{documentName}' not found."
        });
    }

    var document = await documentProvider.GetOpenApiDocumentAsync();

    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    document.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(writer));
    writer.Flush();
    stream.Position = 0;
    var doc = new StreamReader(stream).ReadToEnd();
    doc = doc.Replace("\\n", "\n");

    return TypedResults.Ok(doc);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
