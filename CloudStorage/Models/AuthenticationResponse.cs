using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class AuthenticationResponse
{
    [Required] public string Token { get; init; } = null!;
}