using CloudStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly UserCommand _command;

    public UserController(UserCommand command)
    {
        _command = command;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<UserResponse> CreateUser(UserCreationRequest request)
    {
        var user = await _command.CreateUser(request.Username, request.Password);
        return user.ToResponse();
    }
}