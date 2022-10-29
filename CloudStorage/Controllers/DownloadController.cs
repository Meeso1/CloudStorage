using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    [Route("{id:guid}")]
    public async Task<FileResult?> DownloadFile(Guid id)
    {
        // TODO: Check user ID and compare with file owner ID
        var fileDetails = await _command.GetContentById(id);
        if (fileDetails is not null) return File(fileDetails.Content, "text/plain", fileDetails.File.FileName);

        Response.StatusCode = StatusCodes.Status404NotFound;
        return null;
    }
}