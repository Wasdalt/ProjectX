using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ProjectX.Models;
using ProjectX.Models.ExitProgram;
using ProjectX.Views;
using ReactiveUI;
using Point = Avalonia.Point;

namespace ProjectX.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RectangleModel _rectangleModel;
    public readonly PointerModel PointerModel = new();
    public ResultsModel ResultsModel { get; }
    public PathModel PathModel { get; } = new();
    public SizeWindow SizeWindow { get; } = new();
    public ImageWindow ImageWindow { get; } = new();
    public RectangleModel RectangleModel => _rectangleModel;
    public IPointerEventHandler PointerEventHandler;
    private readonly IShutdownService _shutdownService;
    private static IDialogService _dialogService;
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    public ICommand ShowImageCommand { get; }

    public MainWindowViewModel()
    {
        ResultsModel = new ResultsModel();
        _rectangleModel = new RectangleModel(ResultsModel);
        PointerEventHandler = new PointerEventHandler<MainWindowViewModel>(this);
        _shutdownService = ServiceLocator.ShutdownService;
        CloseCommand = ReactiveCommand.Create(OnCloseCommandExecuted);
        ShowImageCommand = ReactiveCommand.CreateFromTask(ShowImageAsync);
    }

    private Task ShowImageAsync()
    {
        return _dialogService?.ShowImageDialogAsync();
    }
    public static void Initialize(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
    private void OnCloseCommandExecuted()
    {
        _shutdownService.Shutdown();
    }

    public void ResetToDefault()
    {
        _rectangleModel.Left = _rectangleModel.Top = _rectangleModel.Width = _rectangleModel.Height = 0;
        PathModel.OriginalGeometry = new RectangleGeometry(new Rect(0, 0, SizeWindow.Width, SizeWindow.Height));
        PathModel.Data = PathModel.OriginalGeometry;
        PointerModel.StartPosition = default;
        PointerModel.CurrentPosition = default;
    }

    public void WindowResized(double width, double height)
    {
        SizeWindow.PreviousWidth = SizeWindow.Width;
        SizeWindow.PreviousHeight = SizeWindow.Height;
        SizeWindow.Width = width;
        SizeWindow.Height = height;

        PathModel.OriginalGeometry = new RectangleGeometry(new Rect(0, 0, SizeWindow.Width, SizeWindow.Height));
        PathModel.Data = PathModel.OriginalGeometry;

        UpdateRectangleSizeAndPosition();
        CutOutRectangleFromPath(_rectangleModel);
    }

    public void WindowImageCorrect() { }

    public void CutOutRectangleFromPath(RectangleModel rectangleModel)
    {
        var rectangleGeometry = new RectangleGeometry(new Rect(rectangleModel.Left, rectangleModel.Top,
            rectangleModel.Width, rectangleModel.Height));
        var combinedGeometry =
            new CombinedGeometry(GeometryCombineMode.Exclude, PathModel.OriginalGeometry, rectangleGeometry);

        if (_rectangleModel.Width > 0 && _rectangleModel.Height > 0)
        {
            PathModel.Data = combinedGeometry;
        }
    }

    private void UpdateRectangleSizeAndPosition()
    {
        double newX = _rectangleModel.Left / SizeWindow.PreviousWidth * SizeWindow.Width;
        double newY = _rectangleModel.Top / SizeWindow.PreviousHeight * SizeWindow.Height;
        double newWidth = _rectangleModel.Width / SizeWindow.PreviousWidth * SizeWindow.Width;
        double newHeight = _rectangleModel.Height / SizeWindow.PreviousHeight * SizeWindow.Height;

        _rectangleModel.Left = newX;
        _rectangleModel.Top = newY;
        _rectangleModel.Width = newWidth;
        _rectangleModel.Height = newHeight;
    }

    public Point ValidatePosition(Point position)
    {
        double x = Math.Max(0, Math.Min(position.X, SizeWindow.Width));
        double y = Math.Max(0, Math.Min(position.Y, SizeWindow.Height));
        return new Point(x, y);
    }

    public void PointerPressedHandler(object sender, PointerPressedEventArgs args)
    {
        PointerEventHandler.HandlePointerPressed(args);
    }

    public void PointerMovedHandler(object sender, PointerEventArgs args)
    {
        PointerEventHandler.HandlePointerMoved(args);
    }

    public void PointerReleasedHandler(object sender, PointerReleasedEventArgs args)
    {
        PointerEventHandler.HandlePointerReleased(args);
    }

    public void SubscribeToEvents(MainWindow window)
    {
        window.LayoutUpdated += OnLayoutUpdated;
        window.WhenAnyValue(w => w.Bounds)
            .Subscribe(bounds => WindowResized(bounds.Width, bounds.Height));
        window.PositionChanged += (s, e) =>
            SizeWindow.WindowPosition = new PixelPoint(window.Position.X, window.Position.Y);
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        var window = (MainWindow)sender!;
        SizeWindow.Width = window.Width;
        SizeWindow.Height = window.Height;
        WindowResized(SizeWindow.Width, SizeWindow.Height);
        SizeWindow.WindowPosition = new PixelPoint(window.Position.X, window.Position.Y);

        Console.WriteLine($"Initial Size: Width={SizeWindow.Width}, Height={SizeWindow.Height}");
        Console.WriteLine($"Initial Position: X={SizeWindow.WindowPosition.X}, Y={SizeWindow.WindowPosition.Y}");

        WindowImageCorrect();
        window.LayoutUpdated -= OnLayoutUpdated;
    }
}
