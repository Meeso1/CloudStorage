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
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;

        var created = await _command.CreateStoredFile(request.FileName, request.MaxSize, request.Replaceable, userId);
        return created is null ? NotFound() : created.ToResponse();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("upload/{fileId:guid}")]
    public async Task<ActionResult<StoredFileResponse>> UploadFile(Guid fileId, IFormFile file)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;
        var fileDetails = await _command.GetFileDetailsById(fileId);

        if (fileDetails is null) return NotFound();

        if (fileDetails.Owner is not null && fileDetails.Owner.Id != userId) return Unauthorized();

        var result = await _command.UploadFileContent(fileId, file);

        return result is null ? BadRequest() : result.ToResponse();
    }
}