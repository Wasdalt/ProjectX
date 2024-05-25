using System.Threading.Tasks;
using Avalonia.Controls;
using ProjectX.ViewModels.Page;
using ProjectX.Views;

namespace ProjectX.ViewModels;

public interface IDialogService
{
    Task ShowTextDialogAsync();
    Task ShowImageDialogAsync();
}

public class DialogService : IDialogService
{
    private readonly Window _mainWindow;

    public DialogService(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public async Task ShowTextDialogAsync()
    {
        var dialog = new SecondWindow
        {
            DataContext = new SecondWindowViewModel { CurrentPage = new TextPage() }
        };
        await dialog.ShowDialog(_mainWindow);
    }

    public async Task ShowImageDialogAsync()
    {
        var dialog = new SecondWindow
        {
            DataContext = new SecondWindowViewModel { CurrentPage = new ImagePage() }
        };
        await dialog.ShowDialog(_mainWindow);
    }
}