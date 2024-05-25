using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ProjectX.Models;

namespace ProjectX.Views;

// Интерфейс реализующий классы для создания скриншота экрана/экранов
public interface IScreenshotService<T>
{
    string CaptureScreenshot(params T[] args);
    string CropScreenshot(int startX, int startY, int width, int height);
    void CleanupTemporaryImages();
}

public abstract class ScreenshotService : IScreenshotService<double>
{
    protected readonly List<string> TemporaryImagePaths = new();
    protected readonly ScreenshotCropper Cropper;

    protected ScreenshotService()
    {
        Cropper = new ScreenshotCropper(TemporaryImagePaths);
    }

    public abstract string CaptureScreenshot(params double[] args);

    public string CropScreenshot(int startX, int startY, int width, int height)
    {
        return Cropper.CropScreenshot(startX, startY, width, height);
    }

    public void CleanupTemporaryImages()
    {
        foreach (var path in TemporaryImagePaths)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete temporary file {path}: {ex.Message}");
                }
            }
        }
        TemporaryImagePaths.Clear();
    }

    protected string GetScreenshotPath()
    {
        string path = Path.Combine(ProjectPathProvider.AssetsDirectory, "screenshot.png");
        TemporaryImagePaths.Add(path);
        return path;
    }
}


public class WindowsScreenshotService : ScreenshotService
{
    public override string CaptureScreenshot(params double[] args)
    {
        if (args.Length < 4)
        {
            throw new ArgumentException("Insufficient arguments. Provide x, y, width, and height.");
        }

        double x = args[0];
        double y = args[1];
        double width = args[2];
        double height = args[3];

        string savePath = GetScreenshotPath();

        using (var bitmap = new Bitmap((int)width, (int)height))
        {
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen((int)x, (int)y, 0, 0, bitmap.Size);
            }

            bitmap.Save(savePath, ImageFormat.Png);
        }

        return savePath;
    }
}



public class GnomeWaylandScreenshotService : ScreenshotService
{
    public override string CaptureScreenshot(params double[] args)
    {
        string outputPath = GetScreenshotPath();

        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(ProjectPathProvider.CurrentDirectory, "GG", ".venv", "bin", "python3"),
            Arguments = $"{Path.Combine(ProjectPathProvider.CurrentDirectory, "GG", "main2.py")} \"{outputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.Combine(ProjectPathProvider.CurrentDirectory, "GG"),
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (var pythonProcess = new Process())
            {
                pythonProcess.StartInfo = startInfo;
                pythonProcess.Start();
                pythonProcess.WaitForExit();

                if (pythonProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException("Error creating screenshot.");
                }

                if (!File.Exists(outputPath))
                {
                    throw new InvalidOperationException($"Screenshot file not found at path: {outputPath}");
                }

                return outputPath;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create screenshot: {ex.Message}", ex);
        }
    }
}



public class MacScreenshotService : ScreenshotService
{
    public override string CaptureScreenshot(params double[] args)
    {
        // Mac-specific screenshot creation implementation
        //...
        return GetScreenshotPath();
    }
}