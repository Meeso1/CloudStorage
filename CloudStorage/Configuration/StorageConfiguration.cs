namespace CloudStorage.Configuration;

public sealed class StorageConfiguration
{
    public const string SectionName = "Storage";

    public string DirPath { get; init; } = "Data";
}