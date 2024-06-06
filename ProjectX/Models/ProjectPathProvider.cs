using System.IO;

namespace ProjectX.Models;

public static class ProjectPathProvider
{
    private static readonly string _currentDirectory;
    private static readonly string _assetsDirectory;
    private static readonly string _recognitionCodeDirectory;
    private static readonly string _virtualEnvActivationPath;

    static ProjectPathProvider()
    {
        _currentDirectory = Directory.GetCurrentDirectory();
        _assetsDirectory = Path.Combine(_currentDirectory, "Assets");

        if (!Directory.Exists(_assetsDirectory))
        {
            Directory.CreateDirectory(_assetsDirectory);
        }

        _recognitionCodeDirectory = Path.Combine(_currentDirectory, "ScriptPy");

        if (!Directory.Exists(_recognitionCodeDirectory))
        {
            Directory.CreateDirectory(_recognitionCodeDirectory);
        }

        _virtualEnvActivationPath = Path.Combine(_recognitionCodeDirectory, ".venv", "bin", "activate");
    }

    public static string ImagePattern => Path.Combine(_assetsDirectory, "screenshot.png");
    public static string CurrentDirectory => _currentDirectory;
    public static string AssetsDirectory => _assetsDirectory;
    public static string RecognitionCodeDirectory => _recognitionCodeDirectory;
    public static string VirtualEnvActivationPath => _virtualEnvActivationPath;
}

