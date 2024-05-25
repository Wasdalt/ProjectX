using System.IO;

namespace ProjectX.Models;

public static class ProjectPathProvider
{
    private static readonly string _currentDirectory;
    private static readonly string _assetsDirectory;

    static ProjectPathProvider()
    {
        // путь к папки, где хранятся все временные изображения
        _currentDirectory = Directory.GetCurrentDirectory();
        _assetsDirectory = Path.Combine(_currentDirectory, "Assets");

        // Ensure the Assets directory exists
        if (!Directory.Exists(_assetsDirectory))
        {
            Directory.CreateDirectory(_assetsDirectory);
        }
    }

    public static string CurrentDirectory => _currentDirectory;
    public static string AssetsDirectory => _assetsDirectory;
}
