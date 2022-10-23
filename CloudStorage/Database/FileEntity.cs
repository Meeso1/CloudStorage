using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Database;

public sealed class FileEntity
{
    [Required] public Guid Id { get; init; }

    [Required] public string FileName { get; init; } = null!;

    [Required] public string Path { get; init; } = null!;

    public UserEntity? Owner { get; init; }
}