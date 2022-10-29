using CloudStorage.Configuration;
using CloudStorage.Database;
using CloudStorage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace CloudStorage;

public sealed class FileCommand
{
    private readonly IOptionsMonitor<StorageConfiguration> _config;
    private readonly FilesDbContext _context;
    private readonly ILogger _logger;

    public FileCommand(ILogger logger, FilesDbContext context, IOptionsMonitor<StorageConfiguration> config)
    {
        _logger = logger;
        _context = context;
        _config = config;
    }

    public async Task<StoredFile?> CreateStoredFile(string name, long? maxSize, bool replaceable, Guid? ownerId)
    {
        if (ownerId is not null && !_context.Users.Any(u => u.Id == ownerId))
        {
            _logger.Warning("User with id {Username} does not exist. File was not created", ownerId);
            return null;
        }

        _logger.Debug("Creating stored file {FileName}", name);

        var now = DateTimeOffset.Now;
        var entity = new FileEntity
        {
            Id = Guid.NewGuid(),
            FileName = name,
            Path = null,
            OwnerId = ownerId, // TODO: This doesn't link? Investigate
            CreationTime = now,
            LastModificationTime = now,
            Size = 0,
            MaxSize = maxSize,
            Replaceable = replaceable
        };
        _context.Files.Add(entity);
        await _context.SaveChangesAsync();

        return StoredFile.FromEntity(entity);
    }

    public async Task<StoredFile?> UploadFileContent(Guid id, IFormFile file)
    {
        var entity = await _context.Files.Include(f => f.Owner).SingleOrDefaultAsync(f => f.Id == id);
        if (entity is null)
        {
            _logger.Warning("File entity with id {Id} does not exist", id);
            return null;
        }

        if (!entity.Replaceable && entity.Size > 0)
        {
            _logger.Warning("File {Id} already exists and cannot be replaced", id);
            return null;
        }

        if (entity.MaxSize is not null && entity.MaxSize.Value < file.Length)
        {
            _logger.Warning(
                "Cannot write {BytesCount} bytes to File {Id}, because it has a size limit of {Limit} bytes",
                file.Length, id, entity.MaxSize.Value);
            return null;
        }

        if (!Directory.Exists(_config.CurrentValue.DirPath))
        {
            Directory.CreateDirectory(_config.CurrentValue.DirPath);
            _logger.Information("Created storage directory: {Path}", _config.CurrentValue.DirPath);
        }

        var storageFileName = Guid.NewGuid().ToString().Replace("-", "");
        var path = Path.Combine(_config.CurrentValue.DirPath, storageFileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        if (entity.Path is not null) File.Delete(entity.Path);

        entity.Path = path;
        entity.Size = file.Length;
        entity.LastModificationTime = DateTimeOffset.Now;
        await _context.SaveChangesAsync();

        return StoredFile.FromEntity(entity);
    }

    public async Task<FileWithContent?> GetContentById(Guid id)
    {
        var fileData = await _context.Files.Include(f => f.Owner).FirstOrDefaultAsync(e => e.Id == id);
        if (fileData is null) return null;

        var bytes = fileData.Path is not null ? await File.ReadAllBytesAsync(fileData.Path) : Array.Empty<byte>();
        return new FileWithContent(StoredFile.FromEntity(fileData), bytes);
    }

    public async Task<StoredFile?> GetFileDetailsById(Guid id)
    {
        var fileData = await _context.Files.Include(f => f.Owner).FirstOrDefaultAsync(e => e.Id == id);
        return fileData is null ? null : StoredFile.FromEntity(fileData);
    }

    public IEnumerable<StoredFile> GetFilesForUser(User user)
    {
        return _context.Files.Include(f => f.Owner).Where(f => f.Owner == null || f.Owner.Username == user.Username)
            .Select(f => StoredFile.FromEntity(f));
    }
}

public sealed record FileWithContent(StoredFile File, byte[] Content);