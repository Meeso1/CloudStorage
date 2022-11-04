using System.ComponentModel.DataAnnotations;
using CloudStorage.Models;

namespace CloudStorage.Database;

public sealed class AccessLinkEntity
{
    [Key] public Guid Id { get; init; }

    [Required] public FileEntity File { get; init; } = null!;

    [Required] public Guid FileId { get; init; }

    [Required] public AccessType Permissions { get; set; }

    public UserEntity? Owner { get; init; }

    public Guid? OwnerId { get; init; }

    public string? PasswordHash { get; set; }
}