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
        LayoutUpdated += OnLayoutUpdated;
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext!;

        // Update the view model with the current size and position of the window
        viewModel.SizeWindow.Width = Width;
        viewModel.SizeWindow.Height = Height;
        viewModel.WindowResized(Width, Height);
        viewModel.SizeWindow.WindowPosition = new PixelPoint(Position.X, Position.Y);

        // Debugging to check the initial size and position
        Console.WriteLine($"Initial Size: Width={Width}, Height={Height}");
        Console.WriteLine($"Initial Position: X={Position.X}, Y={Position.Y}");
        
        viewModel.WindowImageCorrect();
        // Unsubscribe from LayoutUpdated event after the initial update
        LayoutUpdated -= OnLayoutUpdated;

        // Now subscribe to WhenAnyValue and PositionChanged to start tracking changes
        this.WhenAnyValue(window => window.Bounds)
            .Subscribe(bounds => viewModel.WindowResized(bounds.Width, bounds.Height));

        PositionChanged += (s, e) => { viewModel.SizeWindow.WindowPosition = new PixelPoint(Position.X, Position.Y); };
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