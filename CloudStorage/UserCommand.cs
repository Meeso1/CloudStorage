using CloudStorage.Database;
using CloudStorage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudStorage;

public sealed class UserCommand
{
    private readonly FilesDbContext _context;

    public UserCommand(FilesDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUser(string username, string password)
    {
        var newUser = new User(Guid.NewGuid(), username);
        var hashedPassword = new PasswordHasher<User>().HashPassword(newUser, password);
        _context.Users.Add(new UserEntity
        {
            Id = newUser.Id,
            Username = username,
            PasswordHash = hashedPassword
        });
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task<User?> Authenticate(string username, string password)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (userEntity is null) return null;

        var user = User.FromEntity(userEntity);
        var hasher = new PasswordHasher<User>();
        return hasher.VerifyHashedPassword(user, userEntity.PasswordHash, password) != PasswordVerificationResult.Failed
            ? user
            : null;
    }
}