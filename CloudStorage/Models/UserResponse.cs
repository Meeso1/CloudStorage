using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class UserResponse
{
    [Required] public Guid Id { get; init; }

    [Required] public string Username { get; init; } = null!;
}