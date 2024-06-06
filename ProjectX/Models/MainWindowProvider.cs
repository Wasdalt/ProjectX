using System;
using Avalonia.Controls;

namespace ProjectX.Models;

public sealed class MainWindowProvider
{
    private static readonly Lazy<MainWindowProvider> lazy = new(() => new MainWindowProvider());
    private Window _mainWindow;

    private MainWindowProvider() { }

    public static MainWindowProvider Instance => lazy.Value;

    public void Initialize(Window mainWindow)
    {
        if (_mainWindow == null)
        {
            _mainWindow = mainWindow;
        }
        else
        {
            throw new InvalidOperationException("MainWindow is already initialized.");
        }
    }

    public Window MainWindow => _mainWindow ?? throw new InvalidOperationException("MainWindow is not initialized.");
}
