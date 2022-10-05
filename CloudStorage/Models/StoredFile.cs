using CloudStorage.Database;

namespace CloudStorage;

public class StoredFile
{
    public Guid Id { get; init; }

    public string FileName { get; set; } = null!;

    public string Path { get; set; } = null!;

    public static StoredFile FromEntity(FileEntity entity)
    {
        return new StoredFile
        {
            Id = entity.Id,
            FileName = entity.FileName,
            Path = entity.Path
        };
    }
}