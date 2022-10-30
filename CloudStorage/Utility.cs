using System.Security.Claims;
using CloudStorage.Models;

namespace CloudStorage;

public static class Utility
{
    public static Guid? GetUserId(IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        return claim is not null ? Guid.Parse(claim) : null;
    }

    public static bool IsAllowedForUser(this StoredFile file, Guid? userId)
    {
        return file.Owner is not null && file.Owner.Id != userId;
    }
}