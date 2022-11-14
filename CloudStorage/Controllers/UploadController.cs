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

    /*
     * TODO: [Links]
     * 1. Return usernames not user Ids
     * 2. Timeout and usage (separately download & upload) limit in links
     * 3. A way to invalidate links
     * 4. A way to create links for others by username
     * 5. Return all links for user
     * 6. Get link status
     * 7. Clone link (make a link to a link)
     * 8. Migration & manual tests
     */

    /*
     * TODO: [Other]
     * 1. Unit tests
     * 2. XML doc
     * 3. R# config
     * 4. Encrypt stored files
     * 5. Download/upload as stream
     * 6. Set max storage size (in config)
     * 7. Change password (get disposable link by mail -> require mail on register)
     * 8. Remote files (with potentially different APIs -> links are actual links, not IDs)
     */
}