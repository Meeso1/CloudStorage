using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Database;

public sealed class UserEntity
{
    [Required] public Guid Id { get; init; }

    [Required] public string Username { get; init; } = null!;

    [Required] public string PasswordHash { get; init; } = null!;
}