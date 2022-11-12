using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class CreateFileRequest
{
    [Required] public string FileName { get; init; } = null!;

    public string? Password { get; init; }

    public bool Replaceable { get; init; }

    public long? MaxSize { get; init; }
}