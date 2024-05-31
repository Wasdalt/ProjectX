using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ProjectX.Models;
using ProjectX.ViewModels.Page.RealizationOCR;
using ProjectX.ViewModels.Page.RealizationSaveImage;
using ProjectX.Views;
using ReactiveUI;

namespace ProjectX.ViewModels.Page;
public class SaveFileDialogService
{
    [Obsolete("Obsolete")]
    public async Task<string?> ShowSaveFileDialogAsync(Window window)
    {
        var saveFileDialog = new SaveFileDialog
        {
            DefaultExtension = "png",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Image Files", Extensions = new List<string> { "png", "jpg", "jpeg" } },
                new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
            }
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
                Console.WriteLine($"Файл сохранен по пути: {destinationPath}");
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
    private readonly IPageFactory _pageFactory;
    private readonly SaveFileDialogService _saveFileDialogService;
    private readonly FileSaveService _fileSaveService;
    public ImageCropper ImageCropper { get; set; } = new(); // Класс изображения
    public Interaction<Unit, string?> ShowSaveFileDialog { get; }
    public ReactiveCommand<Unit, Unit> OpenSaveFileCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveToMyPicturesCommand { get; }

    public SecondWindowViewModel()
    {
        _pageFactory = ServiceLocator.Resolve<IPageFactory>();
        _saveFileDialogService = new SaveFileDialogService();
        _fileSaveService = new FileSaveService();
        ShowSaveFileDialog = new Interaction<Unit, string?>();
        OpenSaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
        SaveToMyPicturesCommand = ReactiveCommand.CreateFromTask(SaveToMyPicturesAsync);
    }

    private async Task SaveFileAsync()
    {
        var result = await ShowSaveFileDialog.Handle(Unit.Default);
        if (result != null)
        {
            string filePath = result;
            if (ImageCropper.ImageCropperFromBindingPath != null)
                _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, filePath);
        }
    }

    private Task SaveToMyPicturesAsync()
    {
        if (ImageCropper.ImageCropperFromBindingPath != null)
            _fileSaveService.SaveToMyPictures(ImageCropper.ImageCropperFromBindingPath);
        return Task.CompletedTask;
    }

    private async Task DoSaveFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
    {
        var result = await _saveFileDialogService.ShowSaveFileDialogAsync(window);
        interaction.SetOutput(result);
    }

    public void SubscribeToEvents(SecondWindow window)
    {
        window.WhenActivated(d => { d(ShowSaveFileDialog.RegisterHandler(interaction => DoSaveFileDialogAsync(interaction, window))); });
    }

    public void NavigateToPage<T>(ViewModelBase viewModel) where T : IPage, new()
    {
        CurrentPage = _pageFactory.CreatePage<T>();
    }
}


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