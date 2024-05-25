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
    List<Window> CreateWindows();
    void Cleanup();
}

public class WindowsPlatform : IPlatform
{
    private MainWindowViewModel? _viewModel;
    private readonly Lazy<IScreenshotService<double>> _screenshotService;

    public WindowsPlatform()
    {
        _screenshotService = new Lazy<IScreenshotService<double>>(() => new WindowsScreenshotService());
    }

    public List<Window> CreateWindows()
    {
        RegisterServices();

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
        var screenshotPath = _screenshotService.Value.CaptureScreenshot(
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

    private void RegisterServices()
    {
        ServiceLocator.RegisterService(_screenshotService.Value);
    }

    public void Cleanup()
    {
        _screenshotService.Value.CleanupTemporaryImages();
    }
}

// LinuxPlatform.cs
public class LinuxPlatform : IPlatform
{
    private readonly List<MainWindowViewModel> _viewModels = new();
    private readonly Lazy<IScreenshotService<double>> _screenshotService;

    public LinuxPlatform(IScreenshotService<double> screenshotService)
    {
        _screenshotService = new Lazy<IScreenshotService<double>>(() => screenshotService);
    }

    public List<Window> CreateWindows()
    {
        RegisterServices();
        _screenshotService.Value.CaptureScreenshot();

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
            };

            var pathimage = _screenshotService.Value.CropScreenshot(screen.Bounds.X,
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

    private void RegisterServices()
    {
        ServiceLocator.RegisterService(_screenshotService.Value);
    }

    public void Cleanup()
    {
        _screenshotService.Value.CleanupTemporaryImages();
    }
}

// MacPlatform.cs
public class MacPlatform : IPlatform
{
    private readonly Lazy<IScreenshotService<double>> _screenshotService;

    public MacPlatform()
    {
        _screenshotService = new Lazy<IScreenshotService<double>>(() => new MacScreenshotService());
    }

    public List<Window> CreateWindows()
    {
        RegisterServices();
        // Implement Mac-specific behavior
        throw new NotImplementedException();
    }

    public void Cleanup()
    {
        _screenshotService.Value.CleanupTemporaryImages();
    }

    private void RegisterServices()
    {
        ServiceLocator.RegisterService(_screenshotService.Value);
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
            return new WindowsPlatform();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            if (waylandDisplay != null && IsGnome())
            {
                return new LinuxPlatform(new GnomeWaylandScreenshotService());
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacPlatform();
        }

        throw new PlatformNotSupportedException("Unsupported platform or environment.");
    }

    private static bool IsGnome()
    {
        return Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.Contains("GNOME") ?? false;
    }
}