using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CloudStorage.Controllers;

[ApiController]
[Route("auth")]
public sealed class TokenController : ControllerBase
{
    private readonly UserCommand _command;
    private readonly IConfiguration _configuration;

    public TokenController(UserCommand command, IConfiguration configuration)
    {
        _command = command;
        _configuration = configuration;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> GetTokenAsync(AuthenticationRequest request)
    {
        var user = await _command.AuthenticateAsync(request.Username, request.Password);
        if (user is null) return BadRequest();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new Claim("UserId", user.Id.ToString()),
            new Claim("UserName", user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], null, claims,
            expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

        return new AuthenticationResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }
}