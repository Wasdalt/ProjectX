using System.IO;

namespace ProjectX.Models;

public static class ProjectPathProvider
{
    private static readonly string _currentDirectory;
    private static readonly string _assetsDirectory;
    private static readonly string _imagePattern;

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

        // Путь к шаблону изображений
        _imagePattern = Path.Combine(_assetsDirectory, "screenshot.png");
    }

    public static string ImagePattern => _imagePattern;
    public static string CurrentDirectory => _currentDirectory;
    public static string AssetsDirectory => _assetsDirectory;
}
