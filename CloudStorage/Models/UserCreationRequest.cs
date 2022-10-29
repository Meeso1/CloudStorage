using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class UserCreationRequest
{
    [Required] public string Username { get; init; } = null!;

    [Required] public string Password { get; init; } = null!;
}