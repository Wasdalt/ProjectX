using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using ProjectX.Models;
using ProjectX.Models.Screen;
using ProjectX.Models.SelectionToolCropping;
using ProjectX.ViewModels;
using ProjectX.ViewModels.Page;


namespace ProjectX.Views;

public interface IPlatform
{
    List<Window> CreateWindowsAsync();
    void CleanupAsync();
}

public abstract class PlatformBase : IPlatform
{
    protected IScreenshotService<double> ScreenshotService;
    protected List<MainWindowViewModel> ViewModels = new();

    protected PlatformBase()
    {
        ScreenshotService = ServiceLocator.Resolve<IScreenshotService<double>>();
    }

    public abstract List<Window> CreateWindowsAsync();

    public virtual void CleanupAsync()
    {
        ScreenshotService.CleanupTemporaryImages();
    }

    protected static void AddEscKeyCloseHandler(Window window)
    {
        window.KeyDown += (sender, e) =>
        {
            if (e.Key == Key.Escape)
            {
                window.Close();
            }
        };
    }
}

public class WindowsPlatform : PlatformBase
{
    public override List<Window> CreateWindowsAsync()
    {
        var mainWindow = ScreenManager.Instance.MainWindow;
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

        var viewModel = new MainWindowViewModel();
        ViewModels.Add(viewModel);
        mainWindow.DataContext = viewModel;
        mainWindow.Width = combinedBounds.Width / mainScreen.PixelDensity;
        mainWindow.Height = combinedBounds.Height / mainScreen.PixelDensity;
        mainWindow.Position = new PixelPoint(combinedBounds.X, combinedBounds.Y);
        mainWindow.Topmost = true;
        mainWindow.ExtendClientAreaToDecorationsHint = true;
        mainWindow.ExtendClientAreaTitleBarHeightHint = 0;
        mainWindow.CanResize = false;
        mainWindow.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;

        AddEscKeyCloseHandler(mainWindow);

        var screenshotPath = ScreenshotService.CaptureScreenshot(
            combinedBounds.X,
            combinedBounds.Y,
            combinedBounds.Width,
            combinedBounds.Height
        );

        viewModel.ImageWindow.ImageFromBindingPath = screenshotPath; 
        viewModel.ImageWindow.ImageFromBinding = new Bitmap(screenshotPath);

        mainWindow.Show();

        return new List<Window> { mainWindow };
    }
}

public class LinuxPlatform : PlatformBase
{
    public override List<Window> CreateWindowsAsync()
    {
        var allScreens = ScreenManager.Instance.GetAllScreens();
        var windows = new List<Window>();

        ScreenshotService.CaptureScreenshot();

        foreach (var screen in allScreens)
        {
            var viewModel = new MainWindowViewModel();
            ViewModels.Add(viewModel);

            var window = CreateWindow(viewModel);
            window.Width = screen.Bounds.Width;
            window.Height = screen.Bounds.Height;
            window.Position = screen.Bounds.Position;
            window.WindowState = WindowState.FullScreen;

            viewModel.PointerEventHandler = new PointerEventHandler<MainWindowViewModel>(viewModel, ViewModels);

            window.Closed += (sender, args) =>
            {
                foreach (var w in windows.Where(w => w != window))
                {
                    w.Close();
                }
            };

            AddEscKeyCloseHandler(window);
            windows.Add(window);
        }

        foreach (var screen in allScreens)
        {
            var pathImage = ScreenshotCropper.CropScreenshot(screen.Bounds.X,
                screen.Bounds.Y,
                screen.Bounds.Width, screen.Bounds.Height);

            var viewModel = ViewModels[allScreens.IndexOf(screen)];
            viewModel.ImageWindow.ImageFromBinding = new Bitmap(pathImage);
            viewModel.ImageWindow.ImageFromBindingPath = pathImage;
        }

        foreach (var win in windows)
        {
            win.Show();
        }

        return windows;
    }

    public Window CreateWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        var window = new MainWindow
        {
            DataContext = viewModel
        };
        return window;
    }
}

public class MacPlatform : PlatformBase
{
    public override List<Window> CreateWindowsAsync()
    {
        throw new NotImplementedException();
        // добавить код из файла 10
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
            ServiceLocator.Register<IVirtualEnvActivator>(new WindowsVirtualEnvActivator());
            ServiceLocator.Register<IScreenshotService<double>>(new WindowsScreenshotService());
            return new WindowsPlatform();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            if (waylandDisplay != null && IsGnome())
            {
                ServiceLocator.Register<IVirtualEnvActivator>(new LinuxVirtualEnvActivator());
                ServiceLocator.Register<IScreenshotService<double>>(new GnomeWaylandScreenshotService());
                return new LinuxPlatform();
            }
            else if (Environment.GetEnvironmentVariable("DISPLAY") != null)
            {
                ServiceLocator.Register<IVirtualEnvActivator>(new LinuxVirtualEnvActivator());
                ServiceLocator.Register<IScreenshotService<double>>(new GnomeWaylandScreenshotService());
                return new LinuxPlatform();
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            ServiceLocator.Register<IVirtualEnvActivator>(new MacOSVirtualEnvActivator());
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