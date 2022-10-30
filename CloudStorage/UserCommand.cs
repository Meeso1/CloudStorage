using CloudStorage.Database;
using CloudStorage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CloudStorage;

public sealed class UserCommand
{
    private readonly FilesDbContext _context;
    private readonly ILogger _logger;

    public UserCommand(ILogger logger, FilesDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<User> CreateUserAsync(string username, string password)
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

        _logger.Debug("Created a new user {Username} ({Id})", newUser.Username, newUser.Id);
        return newUser;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (userEntity is null)
        {
            _logger.Warning("User with username {Username} was not found", username);
            return null;
        }

        var user = User.FromEntity(userEntity);
        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, userEntity.PasswordHash, password) != PasswordVerificationResult.Failed)
        {
            _logger.Debug("Authentication failed for user {Username}: password {Password} is incorrect", username,
                password);
            return user;
        }

        return null;
    }
}