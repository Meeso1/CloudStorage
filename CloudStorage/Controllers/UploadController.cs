using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace CloudStorage.Controllers;

[ApiController]
[Route("upload")]
public sealed class UploadController : ControllerBase
{
    private readonly FileCommand _command;
    private readonly ILogger _logger;

    public UploadController(ILogger logger, FileCommand command)
    {
        _logger = logger;
        _command = command;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> UploadFile(UploadFileRequest request)
    {
        var created = await _command.CreateStoredFile(request.FileName, request.Content);
        return created is null ? NotFound() : created.Id;
    }
}