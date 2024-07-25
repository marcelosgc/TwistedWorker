using Microsoft.Extensions.Configuration;
using OneOf;

namespace TwistedCore.Services;

public interface ITwistedService
{
    Task<OneOf<HashSet<string>, Exception>> GetLogFiles();
}

public class TwistedService(IConfiguration configuration) : ITwistedService
{
    public async Task<OneOf<HashSet<string>, Exception>> GetLogFiles()
    {
        var pathLogFolder = configuration.GetSection("LogFolder").Value;

        if (string.IsNullOrWhiteSpace(pathLogFolder) || !Directory.Exists(pathLogFolder) )
            return new DirectoryNotFoundException($"No directory {pathLogFolder} was found.");

        return await GetFileNamesAsync(pathLogFolder);
    }

    private static Task<HashSet<string>> GetFileNamesAsync(string pathLogFolder)
    {
        return Task.Run(() =>
        {
            var files = Directory.EnumerateFiles(pathLogFolder).ToHashSet();
            return files;
        });
    }
}