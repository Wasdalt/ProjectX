using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using ProjectX.Models;
using ProjectX.Models.Screen;
using ProjectX.ViewModels.Page.RealizationOCR;
using ProjectX.ViewModels.Page.Settings;
using ProjectX.Views;
using ReactiveUI;

namespace ProjectX.ViewModels.Page;

// public class SecondWindowViewModel : PageViewModelBase
// {
//     private readonly IPageFactory _pageFactory;
//     private readonly SaveFileDialogService _saveFileDialogService;
//     private readonly FileSaveService _fileSaveService;
//     private readonly SaveTextFileDialogService _saveTextFileDialogService;
//     private readonly TextFileSaveService _textFileSaveService;
//
//     public ImageCropper ImageCropper { get; set; } = new();
//     public Interaction<Unit, string?> ShowSaveFileDialog { get; }
//     public ReactiveCommand<Unit, Unit> OpenSaveFileCommand { get; }
//     public ReactiveCommand<Unit, Unit> SaveToMyPicturesCommand { get; }
//     public Interaction<Unit, string?> ShowSaveTextFileDialog { get; }
//     public ReactiveCommand<Unit, Unit> OpenSaveTextFileCommand { get; }
//     public ReactiveCommand<Unit, Unit> SaveToMyDocumentsCommand { get; }
//     public ReactiveCommand<Unit, Unit> CopyTextToClipboardCommand { get; }
//     public ReactiveCommand<Unit, Unit> SaveConfigCommand { get; }
//     
//     private List<Key> _pressedKeys;
//     private bool _isRecordingKeys;
//     private string _launchKeyCombination;
//
//     public SecondWindowViewModel()
//     {
//         AvailableOCREngines = Enum.GetValues(typeof(OCREngine)).Cast<OCREngine>().ToList();
//         _pageFactory = ServiceLocator.Resolve<IPageFactory>();
//         _saveFileDialogService = new SaveFileDialogService();
//         _fileSaveService = new FileSaveService();
//         ShowSaveFileDialog = new Interaction<Unit, string?>();
//         OpenSaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
//         SaveToMyPicturesCommand = ReactiveCommand.CreateFromTask(SaveToMyPicturesAsync);
//         _saveTextFileDialogService = new SaveTextFileDialogService();
//         _textFileSaveService = new TextFileSaveService();
//         ShowSaveTextFileDialog = new Interaction<Unit, string?>();
//         OpenSaveTextFileCommand = ReactiveCommand.CreateFromTask(SaveTextFileAsync);
//         SaveToMyDocumentsCommand = ReactiveCommand.CreateFromTask(SaveToMyDocumentsAsync);
//         CopyTextToClipboardCommand = ReactiveCommand.CreateFromTask(CopyTextToClipboardAsync);
//         SaveConfigCommand = ReactiveCommand.Create(SaveConfig);
//     }
//
//     
//         public string LaunchKeyCombination
//     {
//         get => _launchKeyCombination;
//         set => this.RaiseAndSetIfChanged(ref _launchKeyCombination, value);
//     }
//
//     public void OnTextBoxGotFocus()
//     {
//         _isRecordingKeys = true;
//     }
//
//     public void OnTextBoxLostFocus()
//     {
//         _isRecordingKeys = false;
//     }
//
//     public void OnTextBoxKeyDown(KeyEventArgs e)
//     {
//         if (_isRecordingKeys)
//         {
//             e.Handled = true; // Prevent default handling of the key event
//
//             if (e.Key == Key.Escape) // Allow user to cancel the recording
//             {
//                 _isRecordingKeys = false;
//                 _pressedKeys.Clear();
//                 LaunchKeyCombination = "Press keys...";
//                 return;
//             }
//
//             // Ensure the first key is a modifier (Shift, Alt, Ctrl)
//             if (_pressedKeys.Count == 0 && !IsModifierKey(e.Key))
//             {
//                 return; // Ignore non-modifier keys if no keys have been recorded yet
//             }
//
//             // If a new modifier key is pressed and we have reached the limit, restart the combination
//             if (_pressedKeys.Count >= 3 && IsModifierKey(e.Key))
//             {
//                 _pressedKeys.Clear();
//             }
//
//             // Add the key to the list of pressed keys
//             if (!_pressedKeys.Contains(e.Key))
//             {
//                 _pressedKeys.Add(e.Key);
//             }
//
//             // Build the combination string
//             var keyCombinationBuilder = new StringBuilder();
//
//             // Append the keys dynamically
//             foreach (var key in _pressedKeys)
//             {
//                 if (keyCombinationBuilder.Length > 0)
//                 {
//                     keyCombinationBuilder.Append(" + ");
//                 }
//                 keyCombinationBuilder.Append(key.ToString());
//             }
//
//             LaunchKeyCombination = keyCombinationBuilder.ToString();
//
//             // Stop recording if more than 3 keys are pressed and not a new modifier key
//             if (_pressedKeys.Count >= 3 && !IsModifierKey(e.Key))
//             {
//                 _isRecordingKeys = false;
//             }
//         }
//     }
//
//     private bool IsModifierKey(Key key)
//     {
//         return key == Key.LeftShift || key == Key.RightShift ||
//                key == Key.LeftCtrl || key == Key.RightCtrl ||
//                key == Key.LeftAlt || key == Key.RightAlt;
//     }
//     private bool _isBusy;
//     public bool IsBusy
//     {
//         get => _isBusy;
//         set => this.RaiseAndSetIfChanged(ref _isBusy, value);
//     }
//
//     private OCREngine _selectedOCREngine;
//     public OCREngine SelectedOCREngine
//     {
//         get => _selectedOCREngine;
//         set
//         {
//             this.RaiseAndSetIfChanged(ref _selectedOCREngine, value);
//             UpdateLanguageOptions();
//         }
//     }
//
//     private string _selectedLanguage = null!;
//     public string SelectedLanguage
//     {
//         get => _selectedLanguage;
//         set => this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
//     }
//
//     private List<OCREngine> _availableOCREngines = null!;
//     public List<OCREngine> AvailableOCREngines
//     {
//         get => _availableOCREngines;
//         set => this.RaiseAndSetIfChanged(ref _availableOCREngines, value);
//     }
//
//     private List<string> _availableLanguages = null!;
//     public List<string> AvailableLanguages
//     {
//         get => _availableLanguages;
//         set => this.RaiseAndSetIfChanged(ref _availableLanguages, value);
//     }
//
//     private void LoadConfig()
//     {
//         var config = AppConfig.Instance;
//         SelectedOCREngine = config.OCREngine;
//         UpdateLanguageOptions();
//         SelectedLanguage = config.OCRLanguage;
//     }
//
//     private void UpdateLanguageOptions()
//     {
//         switch (SelectedOCREngine)
//         {
//             case OCREngine.TesseractOCR:
//                 AvailableLanguages = new List<string> { "rus", "eng", "rus+eng" };
//                 break;
//             case OCREngine.EasyOCR:
//                 AvailableLanguages = new List<string> { "ru", "en" };
//                 break;
//         }
//         SelectedLanguage = AvailableLanguages.Contains(SelectedLanguage) ? SelectedLanguage : AvailableLanguages.FirstOrDefault();
//     }
//
//     private void SaveConfig()
//     {
//         var config = AppConfig.Instance;
//         config.SetOCREngine(SelectedOCREngine);
//         config.SetOCRLanguage(SelectedLanguage);
//     }
//
//     private async Task CopyTextToClipboardAsync()
//     {
//         if (!string.IsNullOrEmpty(ImageCropper.Text))
//         {
//             try
//             {
//                 var clipboard = ScreenManager.Instance.MainWindow.Clipboard;
//                 await clipboard.SetTextAsync(ImageCropper.Text);
//             }
//             catch (InvalidOperationException ex)
//             {
//                 Console.WriteLine($"Error copying to clipboard: {ex.Message}");
//             }
//         }
//     }
//
//     private async Task SaveTextFileAsync()
//     {
//         var result = await ShowSaveTextFileDialog.Handle(Unit.Default);
//         if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(ImageCropper.Text))
//         {
//             _textFileSaveService.SaveTextFile(ImageCropper.Text, result);
//         }
//     }
//
//     private Task SaveToMyDocumentsAsync()
//     {
//         if (!string.IsNullOrEmpty(ImageCropper.Text))
//             _textFileSaveService.SaveToMyDocuments(ImageCropper.Text);
//         return Task.CompletedTask;
//     }
//
//     private async Task DoSaveTextFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
//     {
//         var result = await _saveTextFileDialogService.ShowSaveTextFileDialogAsync(window, "scanned_text.txt");
//         interaction.SetOutput(result);
//     }
//
//     private async Task SaveFileAsync()
//     {
//         var result = await ShowSaveFileDialog.Handle(Unit.Default);
//         if (string.IsNullOrEmpty(result) && ImageCropper.ImageCropperFromBindingPath != null)
//         {
//             string filePath = Path.Combine(
//                 Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
//                 Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
//             _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, filePath);
//         }
//         else if (ImageCropper.ImageCropperFromBindingPath != null)
//         {
//             _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, result);
//         }
//     }
//
//     private Task SaveToMyPicturesAsync()
//     {
//         if (ImageCropper.ImageCropperFromBindingPath != null)
//             _fileSaveService.SaveToMyPictures(ImageCropper.ImageCropperFromBindingPath);
//         return Task.CompletedTask;
//     }
//
//     private async Task DoSaveFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
//     {
//         var result = await _saveFileDialogService.ShowSaveFileDialogAsync(window, Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
//         if (string.IsNullOrEmpty(result) && ImageCropper.ImageCropperFromBindingPath != null)
//         {
//             result = Path.Combine(
//                 Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
//                 Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
//         }
//
//         interaction.SetOutput(result);
//     }
//
//     public void SubscribeToEvents(SecondWindow window)
//     {
//         window.WhenActivated(d =>
//         {
//             d(ShowSaveFileDialog.RegisterHandler(interaction => DoSaveFileDialogAsync(interaction, window)));
//             d(ShowSaveTextFileDialog.RegisterHandler(interaction => DoSaveTextFileDialogAsync(interaction, window)));
//         });
//     }
//
//     public new void NavigateToPage<T>() where T : IPage, new()
//     {
//         CurrentPage = _pageFactory.CreatePage<T>();
//
//         if (typeof(T) == typeof(OCRPage))
//         {
//             StartOCRecognition();
//         }
//         if (typeof(T) == typeof(SettingsPage))
//         {
//             _pressedKeys = new List<Key>();
//             _isRecordingKeys = false;
//             LaunchKeyCombination = "Press keys...";
//             LoadConfig();
//         }
//     }
//
//     private async Task StartOCRecognition()
//     {
//         IsBusy = true;
//         try
//         {
//             ImageCropper.Text = await Task.Run(() =>
//                 PythonScriptExecutor.CaptureScannerText(ImageCropper.ImageCropperFromBindingPath!));
//         }
//         catch (Exception ex)
//         {
//             // Handle exception
//         }
//         finally
//         {
//             IsBusy = false;
//         }
//     }
// }
//

public class SettingsKey : ViewModelBase
{
    private List<Key> _pressedKeys;
    private bool _isRecordingKeys;
    private string _launchKeyCombination;

    private const string PlaceholderText = "Нажмите комбинацию...";

    public string LaunchKeyCombination
    {
        get => string.IsNullOrEmpty(_launchKeyCombination) ? PlaceholderText : _launchKeyCombination;
        set
        {
            var newValue = string.IsNullOrEmpty(value) || value == PlaceholderText ? string.Empty : value;
            this.RaiseAndSetIfChanged(ref _launchKeyCombination, newValue);
        }
    }

    public ReactiveCommand<Unit, Unit> ClearKeyCombinationCommand { get; }

    public SettingsKey()
    {
        _pressedKeys = new List<Key>();
        _isRecordingKeys = false;
        _launchKeyCombination = AppConfig.Instance.LaunchKeyCombination;

        // Initialize the ClearKeyCombinationCommand
        ClearKeyCombinationCommand = ReactiveCommand.Create(ClearKeyCombination);

        if (string.IsNullOrEmpty(_launchKeyCombination))
        {
            _launchKeyCombination = PlaceholderText;
        }
    }

    public void OnTextBoxGotFocus()
    {
        _isRecordingKeys = true;

        // Clear the placeholder text when starting to record keys
        if (_launchKeyCombination == PlaceholderText)
        {
            _launchKeyCombination = string.Empty;
        }
    }

    public void OnTextBoxLostFocus()
    {
        _isRecordingKeys = false;

        // If no keys were recorded, reset to placeholder text
        if (string.IsNullOrEmpty(_launchKeyCombination))
        {
            _launchKeyCombination = PlaceholderText;
        }
    }

    public void OnTextBoxKeyDown(KeyEventArgs e)
    {
        if (_isRecordingKeys)
        {
            e.Handled = true; // Prevent default handling of the key event

            if (e.Key == Key.Escape) // Allow user to cancel the recording
            {
                _isRecordingKeys = false;
                _pressedKeys.Clear();
                LaunchKeyCombination = string.Empty;
                return;
            }

            // Ensure the first key is a modifier (Shift, Alt, Ctrl)
            if (_pressedKeys.Count == 0 && !IsModifierKey(e.Key))
            {
                return; // Ignore non-modifier keys if no keys have been recorded yet
            }

            // If a new modifier key is pressed and we have reached the limit, restart the combination
            if (_pressedKeys.Count >= 3 && IsModifierKey(e.Key))
            {
                _pressedKeys.Clear();
            }

            // Add the key to the list of pressed keys
            if (!_pressedKeys.Contains(e.Key))
            {
                _pressedKeys.Add(e.Key);
            }

            // Build the combination string
            var keyCombinationBuilder = new StringBuilder();

            // Append the keys dynamically
            foreach (var key in _pressedKeys)
            {
                if (keyCombinationBuilder.Length > 0)
                {
                    keyCombinationBuilder.Append(" + ");
                }

                keyCombinationBuilder.Append(key.ToString());
            }

            LaunchKeyCombination = keyCombinationBuilder.ToString();

            // Stop recording if more than 3 keys are pressed and not a new modifier key
            if (_pressedKeys.Count >= 3 && !IsModifierKey(e.Key))
            {
                _isRecordingKeys = false;
            }
        }
    }

    public void ResetKeyRecording()
    {
        _pressedKeys.Clear();
        _isRecordingKeys = false;
        LaunchKeyCombination = string.Empty;
    }

    private bool IsModifierKey(Key key)
    {
        return key == Key.LeftShift || key == Key.RightShift ||
               key == Key.LeftCtrl || key == Key.RightCtrl ||
               key == Key.LeftAlt || key == Key.RightAlt;
    }

    public void SaveKeyCombination()
    {
        AppConfig.Instance.SetLaunchKeyCombination(_launchKeyCombination);
    }

    private void ClearKeyCombination()
    {
        ResetKeyRecording();
    }
}


public class SettingsOcr : ViewModelBase
{
    private OCREngine _selectedOcrEngine;

    public OCREngine SelectedOcrEngine
    {
        get => _selectedOcrEngine;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOcrEngine, value);
            UpdateLanguageOptions();
        }
    }

    private string _selectedLanguage = null!;

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set => this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
    }

    private List<OCREngine> _availableOcrEngines = null!;

    public List<OCREngine> AvailableOcrEngines
    {
        get => _availableOcrEngines;
        set => this.RaiseAndSetIfChanged(ref _availableOcrEngines, value);
    }

    private List<string> _availableLanguages = null!;

    public List<string> AvailableLanguages
    {
        get => _availableLanguages;
        set => this.RaiseAndSetIfChanged(ref _availableLanguages, value);
    }

    public SettingsOcr()
    {
        AvailableOcrEngines = Enum.GetValues(typeof(OCREngine)).Cast<OCREngine>().ToList();
        SelectedOcrEngine = AvailableOcrEngines.FirstOrDefault();
    }

    private void UpdateLanguageOptions()
    {
        switch (SelectedOcrEngine)
        {
            case OCREngine.TesseractOCR:
                AvailableLanguages = new List<string> { "rus", "eng", "eng+rus" };
                break;
            case OCREngine.EasyOCR:
                AvailableLanguages = new List<string> { "ru", "en" };
                break;
            default:
                AvailableLanguages = new List<string>();
                break;
        }

        SelectedLanguage = AvailableLanguages.FirstOrDefault() ?? string.Empty;
    }

    public void LoadConfig()
    {
        var config = AppConfig.Instance;
        SelectedOcrEngine = config.OCREngine;
        SelectedLanguage = config.OCRLanguage;
    }

    public void SaveConfig()
    {
        var config = AppConfig.Instance;
        config.SetOCREngine(SelectedOcrEngine);
        config.SetOCRLanguage(SelectedLanguage);
    }
}

public class SecondWindowViewModel : PageViewModelBase
{
    private readonly IPageFactory _pageFactory;
    public SettingsKey KeySettings { get; }
    public SettingsOcr OcrSettings { get; }

    public ImageCropper ImageCropper { get; set; } = new();
    public Interaction<Unit, string?> ShowSaveFileDialog { get; }
    private readonly SaveTextFileDialogService _saveTextFileDialogService;
    private readonly SaveFileDialogService _saveFileDialogService;
    private readonly FileSaveService _fileSaveService;
    public ReactiveCommand<Unit, Unit> OpenSaveFileCommand { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> SaveToMyPicturesCommand { get; set; } = null!;
    public Interaction<Unit, string?> ShowSaveTextFileDialog { get; }
    public ReactiveCommand<Unit, Unit> OpenSaveTextFileCommand { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> SaveToMyDocumentsCommand { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> CopyTextToClipboardCommand { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> SaveConfigCommand { get; set; } = null!;
    
    public ReactiveCommand<Unit, Unit> ClearKeyCombinationCommand { get; set; } = null!;

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public SecondWindowViewModel()
    {
        _pageFactory = ServiceLocator.Resolve<IPageFactory>();

        KeySettings = new SettingsKey();
        OcrSettings = new SettingsOcr();

        _fileSaveService = new FileSaveService();
        ShowSaveFileDialog = new Interaction<Unit, string?>();
        _saveTextFileDialogService = new SaveTextFileDialogService();
        _saveFileDialogService = new SaveFileDialogService();
        ShowSaveTextFileDialog = new Interaction<Unit, string?>();

        InitializeCommands();
        LoadConfig();
    }

    private void InitializeCommands()
    {
        OpenSaveFileCommand = ReactiveCommand.CreateFromTask(SaveFileAsync);
        SaveToMyPicturesCommand = ReactiveCommand.CreateFromTask(SaveToMyPicturesAsync);
        OpenSaveTextFileCommand = ReactiveCommand.CreateFromTask(SaveTextFileAsync);
        SaveToMyDocumentsCommand = ReactiveCommand.CreateFromTask(SaveToMyDocumentsAsync);
        CopyTextToClipboardCommand = ReactiveCommand.CreateFromTask(CopyTextToClipboardAsync);
        SaveConfigCommand = ReactiveCommand.Create(SaveConfig);
        ClearKeyCombinationCommand = KeySettings.ClearKeyCombinationCommand;
    }

    private void LoadConfig()
    {
        OcrSettings.LoadConfig();
        KeySettings.LaunchKeyCombination = AppConfig.Instance.LaunchKeyCombination;
    }

    private void SaveConfig()
    {
        OcrSettings.SaveConfig();
        KeySettings.SaveKeyCombination();
    }

    private async Task SaveFileAsync()
    {
        var result = await ShowSaveFileDialog.Handle(Unit.Default);
        if (string.IsNullOrEmpty(result) && ImageCropper.ImageCropperFromBindingPath != null)
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Path.GetFileName(result));
            _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, filePath);
        }
        else if (ImageCropper.ImageCropperFromBindingPath != null)
        {
            _fileSaveService.SaveFile(ImageCropper.ImageCropperFromBindingPath, result);
        }
    }
    
    private Task SaveToMyPicturesAsync()
    {
        if (ImageCropper.ImageCropperFromBindingPath != null)
            _fileSaveService.SaveToMyPictures(ImageCropper.ImageCropperFromBindingPath);
        return Task.CompletedTask;
    }

    private async Task SaveTextFileAsync()
    {
        var result = await ShowSaveTextFileDialog.Handle(Unit.Default);
        if (!string.IsNullOrEmpty(result))
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.GetFileName(result));
            if (!string.IsNullOrEmpty(ImageCropper.Text))
            {
                File.WriteAllText(filePath, ImageCropper.Text);
            }
        }
    }

    private Task SaveToMyDocumentsAsync()
    {
        if (!string.IsNullOrEmpty(ImageCropper.Text))
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "scanned_text.txt");
            File.WriteAllText(filePath, ImageCropper.Text);
        }

        return Task.CompletedTask;
    }


    private Task CopyTextToClipboardAsync()
    {
        if (!string.IsNullOrEmpty(ImageCropper.Text))
        {
            var clipboard = ScreenManager.Instance.MainWindow.Clipboard;
            clipboard?.SetTextAsync(ImageCropper.Text);
        }

        return Task.CompletedTask;
    }

    public void SubscribeToEvents(SecondWindow window)
    {
        window.WhenActivated(d =>
        {
            d(ShowSaveFileDialog.RegisterHandler(interaction => DoSaveFileDialogAsync(interaction, window)));
            d(ShowSaveTextFileDialog.RegisterHandler(interaction => DoSaveTextFileDialogAsync(interaction, window)));
        });
    }

    public new void NavigateToPage<T>() where T : IPage, new()
    {
        CurrentPage = _pageFactory.CreatePage<T>();

        if (typeof(T) == typeof(OCRPage))
        {
            StartOcRecognition();
        }

        if (typeof(T) == typeof(SettingsPage))
        {
            LoadConfig();
        }
    }

    private async Task StartOcRecognition()
    {
        IsBusy = true;
        try
        {
            ImageCropper.Text = await Task.Run(() =>
                PythonScriptExecutor.CaptureScannerText(ImageCropper.ImageCropperFromBindingPath!));
        }
        catch (Exception ex)
        {
            // Handle exception
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DoSaveTextFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
    {
        var result = await _saveTextFileDialogService.ShowSaveTextFileDialogAsync(window, "scanned_text.txt");
        interaction.SetOutput(result);
    }

    private async Task DoSaveFileDialogAsync(InteractionContext<Unit, string?> interaction, Window window)
    {
        var result = await _saveFileDialogService.ShowSaveFileDialogAsync(window,
            Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
        if (string.IsNullOrEmpty(result) && ImageCropper.ImageCropperFromBindingPath != null)
        {
            result = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Path.GetFileName(ImageCropper.ImageCropperFromBindingPath));
        }

        interaction.SetOutput(result);
    }
}