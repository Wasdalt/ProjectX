using System;
using System.IO;

namespace ProjectX.ViewModels.Page;

public class TextFileSaveService
{
    public void SaveTextFile(string text, string destinationPath)
    {
        try
        {
            File.WriteAllText(destinationPath, text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении текстового файла: {ex.Message}");
        }
    }

    public void SaveToMyDocuments(string text)
    {
        string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var filePath = Path.Combine(myDocumentsPath, "scanned_text.txt");

        SaveTextFile(text, filePath);
    }
}