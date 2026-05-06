var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use.
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_2;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapMethods("/search", ["QUERY"], (SearchRequest request) =>
{
    return TypedResults.Ok(new { Message = $"You searched for: {request.Query}" });
});

// Other HTTP methods can be mapped using MapMethods as well, but these won't be generated into the OpenAPI document.
app.MapMethods("/notify", ["NOTIFY"], (NotifyRequest request) =>
{
    return TypedResults.Ok(new { Message = $"You notified: {request.Message}" });
});

app.Run();

record SearchRequest(string Query);
record NotifyRequest(string Message);
