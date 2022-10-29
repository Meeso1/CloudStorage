using CloudStorage.Database;

namespace CloudStorage.Models;

public class StoredFile
{
    public Guid Id { get; init; }

    public string FileName { get; init; } = null!;

    public string? Path { get; init; }

    public User? Owner { get; init; }

    public DateTimeOffset CreationTime { get; init; }

    public DateTimeOffset LastModificationTime { get; init; }

    public bool Replaceable { get; init; }

    public long Size { get; init; }

    public long? MaxSize { get; init; }

    public static StoredFile FromEntity(FileEntity entity)
    {
        return new StoredFile
        {
            Id = entity.Id,
            FileName = entity.FileName,
            Path = entity.Path,
            Owner = entity.Owner is null ? null : User.FromEntity(entity.Owner),
            CreationTime = entity.CreationTime,
            LastModificationTime = entity.LastModificationTime,
            Replaceable = entity.Replaceable,
            Size = entity.Size,
            MaxSize = entity.MaxSize
        };
    }

    public StoredFileResponse ToResponse()
    {
        return new StoredFileResponse
        {
            Id = Id,
            FileName = FileName,
            Owner = Owner?.ToResponse(),
            CreationTime = CreationTime,
            LastModificationTime = LastModificationTime,
            Size = Size,
            MaxSize = MaxSize,
            Replaceable = Replaceable
        };
    }
}