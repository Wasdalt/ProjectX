using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using ProjectX.ViewModels.Page;
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

        ServiceLocator.Register<IPageFactory>(new PageFactory());
        _platform = PlatformFactory.CreatePlatform();
        _platform.CreateWindowsAsync();

        // Загружаем конфиг, но не изменяем его
        AppConfig config = AppConfig.Instance;

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        Console.CancelKeyPress += OnCancelKeyPress;

        base.OnFrameworkInitializationCompleted();
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


public enum OCREngine
{
    TesseractOCR,
    EasyOCR
}

public class AppConfig
{
    private static AppConfig _instance = null!;
    private static readonly object _lock = new();

    public OCREngine OCREngine { get; set; }
    public string OCRLanguage { get; set; }
    public string LaunchKeyCombination { get; set; }

    private const string ConfigFileName = "appconfig.json";

    public AppConfig()
    {
        OCREngine = OCREngine.TesseractOCR;
        OCRLanguage = "eng";
        LaunchKeyCombination = string.Empty;
    }

    public static AppConfig Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = LoadConfig();
                }

                return _instance;
            }
        }
    }

    private static AppConfig LoadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            string json = File.ReadAllText(ConfigFileName);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
        else
        {
            AppConfig defaultConfig = new AppConfig();
            defaultConfig.SaveConfig(); // Сохранение конфигурации по умолчанию
            return defaultConfig;
        }
    }

    public void SaveConfig()
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFileName, json);
    }

    public void SetOCREngine(OCREngine engine)
    {
        if (OCREngine != engine)
        {
            OCREngine = engine;
            SaveConfig();
        }
    }

    public void SetOCRLanguage(string language)
    {
        if (OCREngine == OCREngine.TesseractOCR)
        {
            if (language == "rus" || language == "eng" || language == "eng+rus")
            {
                if (OCRLanguage != language)
                {
                    OCRLanguage = language;
                    SaveConfig();
                }
            }
            else
            {
                throw new ArgumentException("Для Tesseract допустимые языки: rus, eng, eng+rus");
            }
        }
        else if (OCREngine == OCREngine.EasyOCR)
        {
            if (language == "ru" || language == "en")
            {
                if (OCRLanguage != language)
                {
                    OCRLanguage = language;
                    SaveConfig();
                }
            }
            else
            {
                throw new ArgumentException("Для EasyOCR допустимые языки: ru, en");
            }
        }
    }

    public string GetPythonScript()
    {
        return OCREngine == OCREngine.TesseractOCR ? "main.py" : "main2.py";
    }

    public void SetLaunchKeyCombination(string keyCombination)
    {
        if (LaunchKeyCombination != keyCombination)
        {
            LaunchKeyCombination = keyCombination;
            SaveConfig();
        }
    }
}

