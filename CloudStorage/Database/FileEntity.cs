using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Database;

public sealed class FileEntity
{
    [Required] public Guid Id { get; init; }

    [Required] public string FileName { get; set; } = null!;

    public string? Path { get; set; }

    public UserEntity? Owner { get; init; }

    public Guid? OwnerId { get; init; }

    [Required] public DateTimeOffset CreationTime { get; init; }

    [Required] public DateTimeOffset LastModificationTime { get; set; }

    [Required] public bool Replaceable { get; set; }

    [Required] public long Size { get; set; }

    public long? MaxSize { get; set; }
}