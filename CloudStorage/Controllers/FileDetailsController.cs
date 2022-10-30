using CloudStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers;

[ApiController]
[Route("files")]
public class FileDetailsController : ControllerBase
{
    private readonly FileCommand _command;

    public FileDetailsController(FileCommand command)
    {
        _command = command;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<StoredFileResponse>> GetAll()
    {
        var userId = Utility.GetUserId(User.Claims);

        IEnumerable<StoredFile> result = await _command.GetDetailsForUserAsync(null);
        if (userId is not null) result = result.Concat(await _command.GetDetailsForUserAsync(userId));

        return result.Select(f => f.ToResponse()).ToList();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:guid}")]
    public async Task<ActionResult<StoredFileResponse>> Get(Guid id)
    {
        var userId = Utility.GetUserId(User.Claims);
        var result = await _command.GetDetailsByIdAsync(id);
        if (result is null) return NotFound();
        if (!result.IsAllowedForUser(userId)) return Unauthorized();

        return result.ToResponse();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = Utility.GetUserId(User.Claims);
        var details = await _command.GetDetailsByIdAsync(id);
        if (details is null) return NotFound();
        if (!details.IsAllowedForUser(userId)) return Unauthorized();

        await _command.DeleteFileAsync(id);
        return Ok();
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<StoredFileResponse>> Update(Guid id, UpdateFileRequest request)
    {
        var userId = Utility.GetUserId(User.Claims);
        var details = await _command.GetDetailsByIdAsync(id);
        if (details is null) return NotFound();
        if (!details.IsAllowedForUser(userId)) return Unauthorized();

        if (request.MaxSize is not null && request.MaxSize < details.Size) return BadRequest();

        var newDetails = await _command.UpdateDetailsAsync(id, request.FileName, request.MaxSize, request.Replaceable);
        if (newDetails is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return newDetails.ToResponse();
    }
}