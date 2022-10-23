using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace CloudStorage.Database;

public sealed class FilesDbContext : DbContext
{
    private readonly ILogger _logger;

    public FilesDbContext(DbContextOptions<FilesDbContext> options, ILogger logger)
        : base(options)
    {
        _logger = logger;
        Database.EnsureCreated();
    }

    public DbSet<FileEntity> Files => Set<FileEntity>();

    public DbSet<UserEntity> Users => Set<UserEntity>();
}