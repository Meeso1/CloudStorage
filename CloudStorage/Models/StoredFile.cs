using CloudStorage.Database;
using CloudStorage.Models;

namespace CloudStorage;

public class StoredFile
{
    public Guid Id { get; init; }

    public string FileName { get; init; } = null!;

    public string Path { get; init; } = null!;

    public User? Owner { get; init; }

    public static StoredFile FromEntity(FileEntity entity)
    {
        return new StoredFile
        {
            Id = entity.Id,
            FileName = entity.FileName,
            Path = entity.Path,
            Owner = entity.Owner is null ? null : User.FromEntity(entity.Owner)
        };
    }
}