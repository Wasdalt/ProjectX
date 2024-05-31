using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    protected readonly ScreenshotCropper Cropper;

    protected ScreenshotService()
    {
        Cropper = new ScreenshotCropper();
    }

    public abstract string CaptureScreenshot(params double[] args);

    public string CropScreenshot(int startX, int startY, int width, int height)
    {
        return Task.Run(() => Cropper.CropScreenshotAsync(startX, startY, width, height)).Result;
    }


    public void CleanupTemporaryImages()
    {
        var temporaryImagePaths = TemporaryImageManager.Instance.TemporaryImagePaths
            .Where(File.Exists)
            .ToList();

        foreach (var path in temporaryImagePaths)
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

        TemporaryImageManager.Instance.Clear();
    }

    protected string GetScreenshotPath()
    {
        string path = ProjectPathProvider.ImagePattern;
        TemporaryImageManager.Instance.Add(path);
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

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "gnome-screenshot",
                Arguments = $"-f \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string errorOutput = process.StandardError.ReadToEnd();
                    throw new InvalidOperationException($"Error creating screenshot: {errorOutput}");
                }

                if (!File.Exists(outputPath))
                {
                    throw new InvalidOperationException($"Screenshot file not found at path: {outputPath}");
                }
            }

            return outputPath;
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

        // For demonstration, we'll just simulate a delay as if we were taking a screenshot.

        return GetScreenshotPath();
    }
}