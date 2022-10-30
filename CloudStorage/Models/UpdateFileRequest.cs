using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class UpdateFileRequest
{
    [Required] public string FileName { get; init; } = null!;

    public bool Replaceable { get; init; }

    public long? MaxSize { get; init; }
}