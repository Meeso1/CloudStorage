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
        if (entity.PasswordHash is not null && hasher.VerifyHashedPassword(link, entity.PasswordHash, password) ==
            PasswordVerificationResult.Failed)
            return new Result<AccessLink>(new UnauthorizedAccessException());

        return new Result<AccessLink>(link);
    }

    public async Task<Result<AccessLink>> CreateLink(Guid fileId, AccessType access, Guid? ownerId = null,
        string? password = null)
    {
        var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file is null) return new Result<AccessLink>(new NotFoundException(fileId));

        var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == ownerId);
        if (owner is null && ownerId is not null) return new Result<AccessLink>(new NotFoundException(ownerId));

        var link = new AccessLink
        {
            Id = Guid.NewGuid(),
            Permissions = access,
            File = StoredFile.FromEntity(file),
            Owner = owner is null ? null : User.FromEntity(owner)
        };

        var hasher = new PasswordHasher<AccessLink>();
        var hashedPassword = hasher.HashPassword(link, password);

        _context.Links.Add(link.ToEntity(hashedPassword));
        await _context.SaveChangesAsync();

        return new Result<AccessLink>(link);
    }
}