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
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;

        IEnumerable<StoredFile> result = await _command.GetFileDetailsForUser(null);
        if (userId is not null) result = result.Concat(await _command.GetFileDetailsForUser(userId));

        return result.Select(f => f.ToResponse()).ToList();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{id:guid}")]
    public async Task<ActionResult<StoredFileResponse>> Get(Guid id)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;

        var result = await _command.GetFileDetailsById(id);
        if (result is null) return NotFound();

        if (result.Owner is not null && result.Owner.Id != userId) return Unauthorized();

        return result.ToResponse();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;

        var result = await _command.GetFileDetailsById(id);
        if (result is null) return Ok();

        if (result.Owner is not null && result.Owner.Id != userId) return Unauthorized();

        await _command.DeleteFile(id);
        return Ok();
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<ActionResult<StoredFileResponse>> Update(Guid id, UpdateFileRequest request)
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var userId = claim is not null ? Guid.Parse(claim) : (Guid?)null;

        var details = await _command.GetFileDetailsById(id);
        if (details is null) return NotFound();

        if (details.Owner is not null && details.Owner.Id != userId) return Unauthorized();

        if (request.MaxSize is not null && request.MaxSize < details.Size) return BadRequest();

        var result = await _command.UpdateFile(id, request.FileName, request.MaxSize, request.Replaceable);
        if (result is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return result.ToResponse();
    }
}