using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyApp.Controllers;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
    // This is the endpoint in the tests
    [HttpPost("filecontentresult")]
    [ProducesResponseType<FileContentResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet)]
    public IActionResult PostFileContentResult()
    {
        var content = "This endpoint returns a FileContentResult!"u8.ToArray();
        return new FileContentResult(content, MediaTypeNames.Application.Octet);
    }

    // FileContentResult with all optional parameters
    [HttpGet("filecontentresult-full")]
    [ProducesResponseType<FileContentResult>(StatusCodes.Status200OK, MediaTypeNames.Text.Plain)]
    public IActionResult GetFileContentResultFull()
    {
        var content = "This endpoint returns a FileContentResult with all optional parameters!"u8.ToArray();

        return new FileContentResult(content, MediaTypeNames.Text.Plain)
        {
            FileDownloadName = "full-example.txt",
            LastModified = new DateTimeOffset(2026, 1, 15, 10, 30, 0, TimeSpan.Zero),
            EntityTag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue("\"unique-etag-value\""),
            EnableRangeProcessing = true
        };
    }
}
