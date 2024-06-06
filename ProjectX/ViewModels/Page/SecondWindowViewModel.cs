using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using ProjectX.Models;
using ProjectX.Models.Screen;
using ProjectX.ViewModels.Page.RealizationOCR;
using ProjectX.ViewModels.Page.RealizationSaveImage;
using ProjectX.Views;
using ReactiveUI;

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

public abstract class PageViewModelBase : ViewModelBase
{
    private IPage _currentPage = null!;

    public IPage CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
}

public class SecondWindowViewModel : PageViewModelBase
{
    // Фабрика страниц
    private readonly IPageFactory _pageFactory;

    // Сервис для сохранения файлов
    private readonly SaveFileDialogService _saveFileDialogService;

    // Сервис для сохранения файлов в папку "Мои изображения"
    private readonly FileSaveService _fileSaveService;
    
    // Модель для кроппинга изображений
    public ImageCropper ImageCropper { get; set; } = new();

    // Интеракция для открытия диалога сохранения файла
    public Interaction<Unit, string?> ShowSaveFileDialog { get; }

    // Команда для открытия диалога сохранения файла
    public ReactiveCommand<Unit, Unit> OpenSaveFileCommand { get; }

    // Команда для сохранения файла в папку "Мои изображения"
    public ReactiveCommand<Unit, Unit> SaveToMyPicturesCommand { get; }
    
    // Сервис для сохранения текстовых файлов
    private readonly SaveTextFileDialogService _saveTextFileDialogService;

    // Сервис для сохранения текстовых файлов в папку "Документы"
    private readonly TextFileSaveService _textFileSaveService;

    // Интеракция для открытия диалога сохранения текстового файла
    public Interaction<Unit, string?> ShowSaveTextFileDialog { get; }

    // Команда для открытия диалога сохранения текстового файла
    public ReactiveCommand<Unit, Unit> OpenSaveTextFileCommand { get; }

    // Команда для сохранения текстового файла в папку "Документы"
    public ReactiveCommand<Unit, Unit> SaveToMyDocumentsCommand { get; }

    // Команда для копирования текста в буфер обмена
    public ReactiveCommand<Unit, Unit> CopyTextToClipboardCommand { get; }
    
    public SecondWindowViewModel()
    {
        _pageFactory = ServiceLocator.Resolve<IPageFactory>();
        _saveFileDialogService = new SaveFileDialogService();
        _fileSaveService = new FileSaveService();
        ShowSaveFileDialog = new Interaction<Unit, string?>();
        OpenSaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
        SaveToMyPicturesCommand = ReactiveCommand.CreateFromTask(SaveToMyPicturesAsync);
        _saveTextFileDialogService = new SaveTextFileDialogService();
        _textFileSaveService = new TextFileSaveService();
        ShowSaveTextFileDialog = new Interaction<Unit, string?>();
        OpenSaveTextFileCommand = ReactiveCommand.CreateFromTask(SaveTextFileAsync);
        SaveToMyDocumentsCommand = ReactiveCommand.CreateFromTask(SaveToMyDocumentsAsync);
        CopyTextToClipboardCommand = ReactiveCommand.CreateFromTask(CopyTextToClipboardAsync);
    }
    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }
    
    // Handler for the CopyTextToClipboardCommand
    private async Task CopyTextToClipboardAsync()
    {
        if (!string.IsNullOrEmpty(ImageCropper.Text))
        {
            try
            {
                var clipboard = ScreenManager.Instance.MainWindow.Clipboard;
                await clipboard.SetTextAsync(ImageCropper.Text);
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where clipboard is not available
                Console.WriteLine($"Error copying to clipboard: {ex.Message}");
            }
        }
    }
    
    // Асинхронное сохранение текстового файла
    private async Task SaveTextFileAsync()
    {
        var result = await ShowSaveTextFileDialog.Handle(Unit.Default);
        if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(ImageCropper.Text))
        {
            _textFileSaveService.SaveTextFile(ImageCropper.Text, result);
        }
    }

    // Асинхронное сохранение текстового файла в папку "Документы"
    private Task SaveToMyDocumentsAsync()
    {
        if (!string.IsNullOrEmpty(ImageCropper.Text))
            _textFileSaveService.SaveToMyDocuments(ImageCropper.Text);
        return Task.CompletedTask;
    }

    // Асинхронное открытие диалога сохранения текстового файла
    private async Task DoSaveTextFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
    {
        var result = await _saveTextFileDialogService.ShowSaveTextFileDialogAsync(window, "scanned_text.txt");
        interaction.SetOutput(result);
    }

    // Асинхронное сохранение файла
    private async Task SaveFileAsync()
    {
        var result = await ShowSaveFileDialog.Handle(Unit.Default);
        if (string.IsNullOrEmpty(result) && ImageCropper.ImageCropperFromBindingPath != null)
        {
            // Если путь к файлу не указан, сохраняем файл в папку "Мои изображения"
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
            _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, filePath);
        }
        else if (ImageCropper.ImageCropperFromBindingPath != null)
        {
            // Если путь к файлу указан, сохраняем файл по указанному пути
            _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, result);
        }
    }

    // Асинхронное сохранение файла в папку "Мои изображения"
    private Task SaveToMyPicturesAsync()
    {
        if (ImageCropper.ImageCropperFromBindingPath != null)
            _fileSaveService.SaveToMyPictures(ImageCropper.ImageCropperFromBindingPath);
        return Task.CompletedTask;
    }

    // Асинхронное открытие диалога сохранения файла
    private async Task DoSaveFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
    {
        var result = await _saveFileDialogService.ShowSaveFileDialogAsync(window, Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
        if (string.IsNullOrEmpty(result) && ImageCropper.ImageCropperFromBindingPath != null)
        {
            // Если путь к файлу не указан, предлагаем сохранить файл в папке "Мои изображения"
            result = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
        }

        interaction.SetOutput(result);
    }

    // Подписка на события окна
    public void SubscribeToEvents(SecondWindow window)
    {
        window.WhenActivated(d =>
        {
            d(ShowSaveFileDialog.RegisterHandler(interaction => DoSaveFileDialogAsync(interaction, window)));
            d(ShowSaveTextFileDialog.RegisterHandler(interaction => DoSaveTextFileDialogAsync(interaction, window)));
        });
    }

    // Навигация на страницу
    public void NavigateToPage<T>() where T : IPage, new()
    {
        CurrentPage = _pageFactory.CreatePage<T>();

        if (typeof(T) == typeof(OCRPage))
        {
            // Открываем страницу OCR
            //...

            // Затем, начинаем процесс распознавания текста
            StartOCRecognition();
        }
    }

    // Начало процесса распознавания текста
    private async Task StartOCRecognition()
    {
        IsBusy = true;
        try
        {
            ImageCropper.Text = await Task.Run(() =>
                PythonScriptExecutor.CaptureScannerText(ImageCropper.ImageCropperFromBindingPath!));
        }
        catch (Exception ex)
        {
            // Handle exceptions (log, display message, etc.)
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public interface IVirtualEnvActivator
{
    void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath);
}

public class WindowsVirtualEnvActivator : IVirtualEnvActivator
{
    public void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath)
    {
        string venvPath = ProjectPathProvider.VirtualEnvActivationPath;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/c \"{venvPath} && python main.py \"{imagePath}\"\"";
    }
}

public class LinuxVirtualEnvActivator : IVirtualEnvActivator
{
    public void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath)
    {
        string venvPath = ProjectPathProvider.VirtualEnvActivationPath;
        startInfo.FileName = "bash";
        startInfo.Arguments = $"-c \"source {venvPath} && python3 main.py \"{imagePath}\"\"";
    }
}

public class MacOSVirtualEnvActivator : IVirtualEnvActivator
{
    public void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath)
    {
        string venvPath = ProjectPathProvider.VirtualEnvActivationPath;
        startInfo.FileName = "bash";
        startInfo.Arguments = $"-c \"source {venvPath} && python3 main.py \"{imagePath}\"\"";
    }
}
public static class PythonScriptExecutor
{
    public static string CaptureScannerText(string imagePath)
    {
        var startInfo = new ProcessStartInfo
        {
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = ProjectPathProvider.RecognitionCodeDirectory
        };

        var activator = ServiceLocator.Resolve<IVirtualEnvActivator>();
        activator.ActivateVirtualEnv(startInfo, imagePath);

        using (var process = Process.Start(startInfo))
        {
            if (process == null)
                throw new Exception("Could not start process");

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
                throw new Exception($"Something went wrong: \n{error}");

            return output;
        }
    }
}


//
//
// public class PythonScriptExecutor
// {
//     public static string CaptureScannerText(string imagePath)
//     {
//         // Указываем путь к виртуальной среде и рабочей директории
//         string venvPath = "/home/nubuck/RiderProjects/ProjectX/ProjectX/bin/Debug/net8.0/GG/.venv/bin/activate";
//         string workingDirectory = "/home/nubuck/RiderProjects/ProjectX/ProjectX/bin/Debug/net8.0/GG";
//         
//         const string cmd = "bash";
//         string activateVenv = $"source {venvPath}";
//         string runPythonScript = $"python3 main.py \"{imagePath}\"";
//
//         var startInfo = new ProcessStartInfo
//         {
//             RedirectStandardOutput = true,
//             RedirectStandardInput = true,
//             RedirectStandardError = true,
//             UseShellExecute = false,
//             CreateNoWindow = true,
//             Arguments = "",
//             FileName = cmd,
//             WorkingDirectory = workingDirectory
//         };
//
//         var process = Process.Start(startInfo);
//         if (process == null)
//             throw new Exception("Could not start process");
//
//         using (var sw = process.StandardInput)
//         {
//             if (sw.BaseStream.CanWrite)
//             {
//                 sw.WriteLine(activateVenv);
//                 sw.WriteLine(runPythonScript);
//                 sw.Flush();
//                 sw.Close();
//             }
//         }
//
//         // Ожидаем завершения процесса
//         process.WaitForExit();
//
//         var output = process.StandardOutput.ReadToEnd();
//         var error = process.StandardError.ReadToEnd();
//
//         if (!string.IsNullOrEmpty(error))
//             throw new Exception($"Something went wrong: \n{error}");
//
//         return output;
//     }
// }

public interface IPage;

public interface IPageFactory
{
    IPage CreatePage<T>() where T : IPage, new();
}

public class PageFactory : IPageFactory
{
    public IPage CreatePage<T>() where T : IPage, new()
    {
        return new T();
    }
}