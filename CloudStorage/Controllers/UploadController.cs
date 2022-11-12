using CloudStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers;

[ApiController]
public sealed class UploadController : ControllerBase
{
    private readonly FileCommand _fileCommand;
    private readonly AccessLinkCommand _linkCommand;

    public UploadController(FileCommand fileCommand, AccessLinkCommand linkCommand)
    {
        _fileCommand = fileCommand;
        _linkCommand = linkCommand;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("create")]
    public async Task<ActionResult<AccessLinkResponse>> CreateFile(CreateFileRequest request)
    {
        var userId = Utility.GetUserId(User.Claims);

        var created =
            await _fileCommand.CreateStoredFileAsync(request.FileName, request.MaxSize, request.Replaceable, userId);
        if (created is null) return NotFound();

        var link = await _linkCommand.CreateLink(created.Id, AccessLink.FullAccess(), userId, request.Password);
        if (link.Exception is not null)
            return link.Exception switch
            {
                NotFoundException => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };

        return AccessLinkResponse.FromLink(link.Value);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("upload/{fileId:guid}")]
    public async Task<ActionResult<AccessLinkResponse>> UploadFile(Guid fileId, IFormFile file)
    {
        var userId = Utility.GetUserId(User.Claims);
        var linkDetails = await _linkCommand.GetLinkDetailsAsync(fileId, userId);
        if (linkDetails.Exception is not null)
            return linkDetails.Exception switch
            {
                NotFoundException => NotFound(),
                UnauthorizedAccessException => Unauthorized(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        if (!linkDetails.Value.Permissions.HasFlag(AccessType.Write)) return Unauthorized();

        var newDetails = await _fileCommand.StoreContentAsync(fileId, file);

        return newDetails is null ? BadRequest() : AccessLinkResponse.FromLink(linkDetails.Value);
    }
}