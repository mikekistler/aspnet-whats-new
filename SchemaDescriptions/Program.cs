using System.ComponentModel;

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

app.MapPost("/", (DescribedInlinedSchemasDto dto) => { });

app.UseHttpsRedirection();

app.Run();

[Description("Class: DescribedInlinedSchemasDto")]
public class DescribedInlinedSchemasDto
{
    [Description("Property: DescribedInlinedSchemasDto.Inlined1")]
    public DescribedInlinedDto Inlined1 { get; set; }

    [Description("Property: DescribedInlinedSchemasDto.Inlined2")]
    public DescribedInlinedDto Inlined2 { get; set; }

    public DescribedInlinedDto InlinedNoDescription { get; set; }
}

[Description("Class: DescribedInlinedDto")]
public class DescribedInlinedDto
{
    [Description("Property: DescribedInlinedDto.ChildValue")]
    public string ChildValue { get; set; }
}