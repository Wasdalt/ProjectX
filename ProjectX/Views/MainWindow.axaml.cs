using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ProjectX.ViewModels;
using ProjectX.ViewModels.Page;
using ReactiveUI;

namespace ProjectX.Views;

public partial class MainWindow :  ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += (s, e) =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.SubscribeToEvents(this);
            }
        };
        this.WhenActivated(d =>
        {
            d(ViewModel!.ShowDialogForSecondWindow.RegisterHandler(DoShowDialogAsync<SecondWindow, SecondWindowViewModel>));
            // d(ViewModel!.ShowDialogForNewWindow.RegisterHandler(DoShowDialogAsync<NewWindow, NewWindowViewModel>));
        });
    }
    private async Task DoShowDialogAsync<TWindow, TViewModel>(InteractionContext<TViewModel, bool> interaction)
        where TWindow : Window, new()
        where TViewModel : ViewModelBase
    {
        var dialog = new TWindow();
        dialog.DataContext = interaction.Input;
        var result = await dialog.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        (DataContext as MainWindowViewModel)?.PointerPressedHandler(sender, e);
    }

    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        (DataContext as MainWindowViewModel)?.PointerMovedHandler(sender, e);
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        (DataContext as MainWindowViewModel)?.PointerReleasedHandler(sender, e);
    }
}
