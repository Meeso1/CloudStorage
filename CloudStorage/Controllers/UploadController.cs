using CloudStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers;

[ApiController]
public sealed class UploadController : ControllerBase
{
    private readonly FileCommand _command;

    public UploadController(FileCommand command)
    {
        _command = command;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("create")]
    public async Task<ActionResult<StoredFileResponse>> CreateFile(CreateFileRequest request)
    {
        var userId = Utility.GetUserId(User.Claims);

        var created =
            await _command.CreateStoredFileAsync(request.FileName, request.MaxSize, request.Replaceable, userId);
        return created is null ? NotFound() : created.ToResponse();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("upload/{fileId:guid}")]
    public async Task<ActionResult<StoredFileResponse>> UploadFile(Guid fileId, IFormFile file)
    {
        var userId = Utility.GetUserId(User.Claims);
        var fileDetails = await _command.GetDetailsByIdAsync(fileId);
        if (fileDetails is null) return NotFound();
        if (!fileDetails.IsAllowedForUser(userId)) return Unauthorized();

        var newDetails = await _command.StoreContentAsync(fileId, file);

        return newDetails is null ? BadRequest() : newDetails.ToResponse();
    }
}