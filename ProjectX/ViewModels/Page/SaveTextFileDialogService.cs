using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace ProjectX.ViewModels.Page;

public class SaveTextFileDialogService
{
    public async Task<string?> ShowSaveTextFileDialogAsync(Window window, string? defaultFileName = null)
    {
        var saveFileDialog = new SaveFileDialog
        {
            DefaultExtension = "txt",
            Filters = new List<FileDialogFilter>
            {
                new() { Name = "Text Files", Extensions = new List<string> { "txt" } },
                new() { Name = "Word Documents", Extensions = new List<string> { "docx", "doc" } },
                new() { Name = "All Files", Extensions = new List<string> { "*" } }
            },
            InitialFileName = defaultFileName
        };

        return await saveFileDialog.ShowAsync(window);
    }
}