using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using ProjectX.Views;

namespace ProjectX;

public class App : Application
{
    private IPlatform _platform = null!;
    private static Mutex? _mutex;
    private static bool _mutexCreated;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _mutex = new Mutex(true, "AvaloniaAppMutex", out _mutexCreated);

        if (!_mutexCreated)
        {
            // Программа уже запущена
            ServiceLocator.ShutdownService.Shutdown();
        }

        _platform = PlatformFactory.CreatePlatform();
        var windows = _platform.CreateWindowsAsync();

        foreach (var window in windows)
        {
            window.Show();
        }

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        Console.CancelKeyPress += OnCancelKeyPress;
    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        Cleanup();
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        Cleanup();
    }

    private void Cleanup()
    {
        _platform.CleanupAsync();
        if (_mutexCreated && _mutex != null)
        {
            try
            {
                // Проверяем, активно ли еще приложение
                if (!_mutex.WaitOne(TimeSpan.Zero))
                {
                    // Если mutex не был захвачен, значит, приложение уже завершилось
                    return;
                }

                _mutex.ReleaseMutex();
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine($"Failed to release mutex: {ex.Message}");
            }
        }
    }
} 