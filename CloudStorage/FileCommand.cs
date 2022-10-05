﻿using System.Text;
using CloudStorage.Configuration;
using CloudStorage.Database;
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

    public async Task<StoredFile> CreateStoredFile(string name, string content)
    {
        var guid = Guid.NewGuid();
        var fullPath = Path.Combine(_config.CurrentValue.DirPath, guid.ToString().Replace("-", "") + name);

        if (!Directory.Exists(_config.CurrentValue.DirPath))
        {
            Directory.CreateDirectory(_config.CurrentValue.DirPath);
            _logger.Information("Created storage directory: {Path}", _config.CurrentValue.DirPath);
        }

        _logger.Debug("Creating stored file {FileName} with id {Id} (path: {Path})", name, guid, fullPath);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await stream.WriteAsync(Encoding.ASCII.GetBytes(content));

        var entity = new FileEntity
        {
            Id = guid,
            FileName = name,
            Path = fullPath
        };
        _context.Files.Add(entity);
        await _context.SaveChangesAsync();

        return StoredFile.FromEntity(entity);
    }

    public async Task<FileWithContent?> GetFileById(Guid id)
    {
        var fileData = await _context.Files.FirstOrDefaultAsync(e => e.Id == id);
        if (fileData is null) return null;

        var bytes = await File.ReadAllBytesAsync(fileData.Path);
        return new FileWithContent(StoredFile.FromEntity(fileData), bytes);
    }
}

public sealed record FileWithContent(StoredFile File, byte[] Content);