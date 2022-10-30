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
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;

        var details = await _command.GetDetailsByIdAsync(id);
        if (details is null) return NotFound();

        if (details.Owner is not null && details.Owner.Id != userId) return Unauthorized();

        var content = await _command.GetContentByIdAsync(id);
        if (content is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return File(content, "text/plain", details.FileName);
    }
}