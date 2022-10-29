using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class StoredFileResponse
{
    [Required] public Guid Id { get; init; }

    [Required] public string FileName { get; init; } = null!;

    public UserResponse? Owner { get; init; }

    [Required] public DateTimeOffset CreationTime { get; init; }

    [Required] public DateTimeOffset LastModificationTime { get; init; }

    [Required] public long Size { get; init; }

    public long? MaxSize { get; init; }

    public bool Replaceable { get; init; }
}