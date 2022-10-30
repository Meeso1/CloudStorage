using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers;

[ApiController]
[Route("download")]
public sealed class DownloadController : ControllerBase
{
    private readonly FileCommand _command;

    public DownloadController(FileCommand command)
    {
        _command = command;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:guid}")]
    public async Task<ActionResult<FileResult>> DownloadFile(Guid id)
    {
        var userId = Utility.GetUserId(User.Claims);
        var details = await _command.GetDetailsByIdAsync(id);
        if (details is null) return NotFound();
        if (!details.IsAllowedForUser(userId)) return Unauthorized();

        var content = await _command.GetContentByIdAsync(id);
        if (content is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return File(content, "text/plain", details.FileName);
    }
}