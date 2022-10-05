using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace CloudStorage.Controllers;

[ApiController]
[Route("download")]
public sealed class DownloadController : ControllerBase
{
    private readonly FileCommand _command;
    private readonly ILogger _logger;

    public DownloadController(ILogger logger, FileCommand command)
    {
        _logger = logger;
        _command = command;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<FileResult?> DownloadFile(Guid id)
    {
        var fileDetails = await _command.GetFileById(id);
        if (fileDetails is not null) return File(fileDetails.Content, "text/plain", fileDetails.File.FileName);

        Response.StatusCode = StatusCodes.Status404NotFound;
        return null;
    }
}