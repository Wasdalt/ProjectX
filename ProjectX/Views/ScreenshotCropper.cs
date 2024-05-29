using System;
using System.IO;
using ProjectX.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ProjectX.Views;
public class ScreenshotCropper
{
    public string CropScreenshot(int startX, int startY, int width, int height)
    {
        string outputPath = Path.Combine(ProjectPathProvider.AssetsDirectory,
            $"screenshot_cropped_{startX}_{startY}_{width}_{height}.png");

        try
        {
            string inputPath = ProjectPathProvider.ImagePattern;

            using (Image image = Image.Load(inputPath))
            {
                Rectangle cropArea = new Rectangle(startX, startY, width, height);
                image.Mutate(x => x.Crop(cropArea));
                image.Save(outputPath);
                TemporaryImageManager.Instance.Add(outputPath);
                return outputPath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return null!;
        }
    }
}

public class ScreenshotCropperWindow
{
    public static string CropScreenshotWindow(string imagepath, int startX, int startY, int width, int height)
    {
        string outputPath = Path.Combine(ProjectPathProvider.AssetsDirectory, "screenshot_cropped_window.png");

        try
        {
            string inputPath = Path.Combine(ProjectPathProvider.AssetsDirectory, imagepath);

            using (Image image = Image.Load(inputPath))
            {
                Rectangle cropArea = new Rectangle(startX, startY, width, height);
                image.Mutate(x => x.Crop(cropArea));
                image.Save(outputPath);
                TemporaryImageManager.Instance.Add(outputPath);
                return Path.GetFileName(outputPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return null!;
        }
    }
}



// public Bitmap CropImage(Bitmap originalImage, int startX, int startY, int width, int height)
// {
//     // Преобразуем Avalonia Bitmap в объект Image из SixLabors.ImageSharp
//     using (var stream = new MemoryStream())
//     {
//         originalImage.Save(stream);
//         stream.Position = 0;
//         using (var image = Image.Load<Rgba32>(stream))
//         {
//             // Обрезаем изображение
//             image.Mutate(x => x.Crop(new Rectangle(startX, startY, width, height)));
//
//             // Преобразуем обрезанное изображение обратно в Avalonia Bitmap
//             using (var croppedStream = new MemoryStream())
//             {
//                 image.Save(croppedStream, PngFormat.Instance);
//                 croppedStream.Position = 0;
//                 var croppedBitmap = new Bitmap(croppedStream);
//
//                 // Заменяем исходное изображение обрезанным
//                 return croppedBitmap;
//             }
//         }
//     }
// }