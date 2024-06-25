using System;
using System.IO;

namespace ProjectX.ViewModels.Page;

public class FileSaveService
{
    public void SaveFile(string sourcePath, string destinationPath)
    {
        if (File.Exists(sourcePath))
        {
            try
            {
                File.Copy(sourcePath, destinationPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Исходный файл не найден.");
        }
    }

    public void SaveToMyPictures(string sourcePath)
    {
        string myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        var filePath = Path.Combine(myPicturesPath, Path.GetFileName(sourcePath)!);

        SaveFile(sourcePath, filePath);
    }
}