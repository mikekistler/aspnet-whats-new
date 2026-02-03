using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Serve the OpenAPI document as a YAML file
    app.MapOpenApi("/openapi/{documentName}.yaml");
}

app.UseHttpsRedirection();

app.MapControllers();

// Minimal API endpoints equivalent to FileController

app.MapPost("/filecontentresult", () =>
{
    var content = "This endpoint returns a FileContentResult!"u8.ToArray();
    return TypedResults.Bytes(content);
})
.Produces<byte[]>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet);

app.MapGet("/filecontentresult-full", () =>
{
    var content = "This endpoint returns a FileContentResult with all optional parameters!"u8.ToArray();
    return TypedResults.File(
        content,
        MediaTypeNames.Text.Plain,
        fileDownloadName: "full-example.txt",
        enableRangeProcessing: true,
        lastModified: new DateTimeOffset(2026, 1, 15, 10, 30, 0, TimeSpan.Zero),
        entityTag: new Microsoft.Net.Http.Headers.EntityTagHeaderValue("\"unique-etag-value\""));
})
.Produces<string>(StatusCodes.Status200OK, MediaTypeNames.Text.Plain);

app.Run();
