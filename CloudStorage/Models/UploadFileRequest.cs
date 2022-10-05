using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Controllers;

public sealed class UploadFileRequest
{
    [Required] public string FileName { get; init; } = null!;

    public string Content { get; init; } = string.Empty;
}