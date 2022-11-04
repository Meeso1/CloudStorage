using CloudStorage.Database;
using CloudStorage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudStorage;

public sealed class AccessLinkCommand
{
    private readonly FilesDbContext _context;

    public AccessLinkCommand(FilesDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AccessLink>> GetLinkDetailsAsync(Guid linkId, Guid? userId, string? password = null)
    {
        var entity = await _context.Links.Include(e => e.File)
            .ThenInclude(f => f.Owner)
            .Include(e => e.Owner)
            .FirstOrDefaultAsync(e => e.Id == linkId);
        if (entity is null) return new Result<AccessLink>(new NotFoundException(linkId));

        var link = AccessLink.FromEntity(entity);
        if (link.Owner is not null && link.Owner.Id != userId)
            return new Result<AccessLink>(new UnauthorizedAccessException());

        var hasher = new PasswordHasher<AccessLink>();
        if (link.PasswordHash is not null && hasher.VerifyHashedPassword(link, link.PasswordHash, password) ==
            PasswordVerificationResult.Failed)
            return new Result<AccessLink>(new UnauthorizedAccessException());

        return new Result<AccessLink>(link);
    }
}