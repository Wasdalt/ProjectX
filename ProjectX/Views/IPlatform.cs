using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ProjectX.Models;
using ProjectX.Models.Screen;
using ProjectX.ViewModels;


namespace ProjectX.Views;

public interface IPlatform
{
    List<Window> CreateWindowsAsync();
    void CleanupAsync();
}

public class WindowsPlatform : IPlatform
{
    private MainWindowViewModel? _viewModel;
    private readonly IScreenshotService<double> _screenshotService;

    public WindowsPlatform()
    {
        _screenshotService = ServiceLocator.Resolve<IScreenshotService<double>>();
    }

    public List<Window> CreateWindowsAsync()
    {
        var allScreens = ScreenManager.Instance.GetAllScreens();
        var mainScreen = allScreens.FirstOrDefault(s => s.Bounds.X <= 0 && s.Bounds.Y <= 0);
        if (mainScreen == null)
        {
            throw new InvalidOperationException("Main screen not found.");
        }

        var combinedBounds = new PixelRect(0, 0, 0, 0);
        foreach (var screen in allScreens)
        {
            combinedBounds = combinedBounds.Union(screen.Bounds);
        }

        _viewModel = new MainWindowViewModel();
        var screenshotPath = _screenshotService.CaptureScreenshot(
            combinedBounds.X,
            combinedBounds.Y,
            combinedBounds.Width,
            combinedBounds.Height
        );

        var window = new MainWindow
        {
            DataContext = _viewModel,
            Width = combinedBounds.Width / mainScreen.PixelDensity,
            Height = combinedBounds.Height / mainScreen.PixelDensity,
            Position = new PixelPoint(combinedBounds.X, combinedBounds.Y),
            Topmost = true,
            ExtendClientAreaToDecorationsHint = true,
            ExtendClientAreaTitleBarHeightHint = 0,
            CanResize = false,
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome
        };

        _viewModel.ImageWindow.ImageFromBinding = new Bitmap(screenshotPath);
        return new List<Window> { window };
    }

    public void CleanupAsync()
    {
        _screenshotService.CleanupTemporaryImages();
    }
}

public class LinuxPlatform : IPlatform
{
    private readonly List<MainWindowViewModel> _viewModels = new();
    private readonly IScreenshotService<double> _screenshotService;

    public LinuxPlatform()
    {
        _screenshotService = ServiceLocator.Resolve<IScreenshotService<double>>();
    }

    public List<Window> CreateWindowsAsync()
    {
        _screenshotService.CaptureScreenshot();

        var windows = new List<Window>();
        var allScreens = ScreenManager.Instance.GetAllScreens();

        foreach (var screen in allScreens)
        {
            var viewModel = new MainWindowViewModel();
            _viewModels.Add(viewModel);

            var window = new MainWindow
            {
                DataContext = viewModel,
                Width = screen.Bounds.Width,
                Height = screen.Bounds.Height,
                Position = screen.Bounds.Position,
                // WindowState = WindowState.FullScreen
            };

            var pathimage = _screenshotService.CropScreenshot(screen.Bounds.X,
                screen.Bounds.Y,
                screen.Bounds.Width, screen.Bounds.Height);
            viewModel.ImageWindow.ImageFromBinding = new Bitmap(pathimage);
            viewModel.ImageWindow.ImageFromBindingPath = pathimage;

            viewModel.PointerEventHandler = new PointerEventHandler<MainWindowViewModel>(viewModel, _viewModels);

            window.Closed += (sender, args) =>
            {
                foreach (var w in windows.Where(w => w != window))
                {
                    w.Close();
                }
            };

            windows.Add(window);
        }

        return windows;
    }

    public void CleanupAsync()
    {
        _screenshotService.CleanupTemporaryImages();
    }
}

public class MacPlatform : IPlatform
{
    private readonly IScreenshotService<double> _screenshotService;

    public MacPlatform()
    {
        _screenshotService = ServiceLocator.Resolve<IScreenshotService<double>>();
    }

    public List<Window> CreateWindowsAsync()
    {
        // Implement Mac-specific behavior
        throw new NotImplementedException();
    }

    public void CleanupAsync()
    {
        _screenshotService.CleanupTemporaryImages();
    }
}

public static class PlatformFactory
{
    private static readonly Lazy<IPlatform> Platform = new(() => CreatePlatformInternal());

    public static IPlatform CreatePlatform()
    {
        return Platform.Value;
    }

    private static IPlatform CreatePlatformInternal()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ServiceLocator.Register<IScreenshotService<double>>(new WindowsScreenshotService());
            return new WindowsPlatform();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            if (waylandDisplay != null && IsGnome())
            {
                ServiceLocator.Register<IScreenshotService<double>>(new GnomeWaylandScreenshotService());
                return new LinuxPlatform();
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ServiceLocator.Register<IScreenshotService<double>>(new MacScreenshotService());
            return new MacPlatform();
        }

        throw new PlatformNotSupportedException("Unsupported platform or environment.");
    }

    private static bool IsGnome()
    {
        return Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.Contains("GNOME") ?? false;
    }
}