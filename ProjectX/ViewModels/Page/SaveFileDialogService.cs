using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace ProjectX.ViewModels.Page;

public class SaveFileDialogService
{
    public async Task<string?> ShowSaveFileDialogAsync(Window window, string? defaultFileName = null)
    {
        var saveFileDialog = new SaveFileDialog
        {
            DefaultExtension = "png",
            Filters = new List<FileDialogFilter>
            {
                new() { Name = "Image Files", Extensions = new List<string> { "png", "jpg", "jpeg" } },
                new() { Name = "All Files", Extensions = new List<string> { "*" } }
            },
            InitialFileName = defaultFileName
        };

        return await saveFileDialog.ShowAsync(window);
    }
}