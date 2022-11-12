using System.ComponentModel.DataAnnotations;

namespace CloudStorage.Models;

public sealed class AccessLinkResponse
{
    [Required] public Guid Id { get; init; }

    public UserResponse? Owner { get; init; }

    [Required] public string FileName { get; init; } = null!;

    [Required] public AccessType Permissions { get; init; }

    public static AccessLinkResponse FromLink(AccessLink link)
    {
        return new AccessLinkResponse
        {
            Id = link.Id,
            FileName = link.File.FileName,
            Permissions = link.Permissions,
            Owner = link.Owner?.ToResponse()
        };
    }
}