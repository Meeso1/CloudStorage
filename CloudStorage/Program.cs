using CloudStorage;
using CloudStorage.Configuration;
using CloudStorage.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());

var services = builder.Services;

services.AddScoped<FileCommand>();
services.AddScoped<UserCommand>();

services.Configure<StorageConfiguration>(builder.Configuration.GetSection(StorageConfiguration.SectionName));
services.AddDbContext<FilesDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Files")));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();