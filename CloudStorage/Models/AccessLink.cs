using CloudStorage.Database;

namespace CloudStorage.Models;

public sealed class AccessLink
{
    public Guid Id { get; init; }

    public StoredFile File { get; init; } = null!;

    public AccessType Permissions { get; set; }

    public User? Owner { get; init; }

    public string? PasswordHash { get; set; }

    public AccessLinkEntity ToEntity()
    {
        return new()
        {
            Id = Id,
            FileId = File.Id,
            OwnerId = Owner?.Id,
            Permissions = Permissions,
            PasswordHash = PasswordHash
        };
    }

    public static AccessLink FromEntity(AccessLinkEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            File = StoredFile.FromEntity(entity.File),
            Owner = entity.Owner is null ? null : User.FromEntity(entity.Owner),
            Permissions = entity.Permissions,
            PasswordHash = entity.PasswordHash
        };
    }
}

[Flags]
public enum AccessType
{
    Read = 1,
    Write = 2,
    Modify = 4
}