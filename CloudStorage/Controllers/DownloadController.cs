using CloudStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers;

[ApiController]
[Route("download")]
public sealed class DownloadController : ControllerBase
{
    private readonly FileCommand _fileCommand;
    private readonly AccessLinkCommand _linkCommand;

    public DownloadController(FileCommand fileCommand, AccessLinkCommand linkCommand)
    {
        _fileCommand = fileCommand;
        _linkCommand = linkCommand;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:guid}")]
    public async Task<ActionResult<FileResult>> DownloadFile(Guid id)
    {
        var userId = Utility.GetUserId(User.Claims);
        var linkDetails = await _linkCommand.GetLinkDetailsAsync(id, userId);
        if (linkDetails.Exception is not null)
            return linkDetails.Exception switch
            {
                NotFoundException => NotFound(),
                UnauthorizedAccessException => Unauthorized(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        if (!linkDetails.Value.Permissions.HasFlag(AccessType.Read)) return Unauthorized();

        var content = await _fileCommand.GetContentByIdAsync(linkDetails.Value.File.Id);
        if (content is null) return NotFound();

        return File(content, "text/plain", linkDetails.Value.File.FileName);
    }
}