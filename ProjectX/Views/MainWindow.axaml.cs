using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var dialogService = new DialogService(this);
        MainWindowViewModel.Initialize(dialogService);
        DataContextChanged += (s, e) =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.SubscribeToEvents(this);
            }
        };
    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PointerPressedHandler(sender, e);
        }
    }

    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PointerMovedHandler(sender, e);
        }
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PointerReleasedHandler(sender, e);
        }
    }
}
