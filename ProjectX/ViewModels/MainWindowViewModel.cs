using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using ProjectX.Models;
using ReactiveUI;
using Point = Avalonia.Point;

namespace ProjectX.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RectangleModel _rectangleModel = new();
    public readonly PointerModel PointerModel = new();
    public ResultsModel ResultsModel { get; } = new();
    public PathModel PathModel { get; } = new();
    public SizeWindow SizeWindow { get; } = new();
    public ImageWindow ImageWindow { get; } = new();
    public RectangleModel RectangleModel => _rectangleModel;
    public IPointerEventHandler PointerEventHandler;
    public ReactiveCommand<Unit, Unit> ShowTextCommand { get; }
    private static IDialogService _dialogService;
    public MainWindowViewModel()
    {
        PointerEventHandler = new PointerEventHandler<MainWindowViewModel>(this);
        ShowTextCommand = ReactiveCommand.CreateFromTask(ShowTextAsync);
    }

    public static void Initialize(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
    
    private Task ShowTextAsync()
    {
        return _dialogService?.ShowTextDialogAsync();
    }
    public void ResetToDefault()
    {
        // Reset rectangle coordinates to default
        _rectangleModel.Left = _rectangleModel.Top = _rectangleModel.Width = _rectangleModel.Height = 0;

        // Reset path to original state (full window)
        PathModel.OriginalGeometry = new RectangleGeometry(new Rect(0, 0, SizeWindow.Width, SizeWindow.Height));
        PathModel.Data = PathModel.OriginalGeometry;
    }

    // Window resized handler
    public void WindowResized(double width, double height)
    {
        SizeWindow.PreviousWidth = SizeWindow.Width;
        SizeWindow.PreviousHeight = SizeWindow.Height;
        SizeWindow.Width = width;
        SizeWindow.Height = height;

        // Set path to full window
        PathModel.OriginalGeometry = new RectangleGeometry(new Rect(0, 0, SizeWindow.Width, SizeWindow.Height));
        PathModel.Data = PathModel.OriginalGeometry;

        // Update rectangle size and position based on new window size
        UpdateRectangleSizeAndPosition();

        // Update path model only if the rectangle has non-zero size
        CutOutRectangleFromPath(_rectangleModel);
    }

    public void WindowImageCorrect()
    {
    }

    // Update path with rectangle exclusion
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

    // Update rectangle size and position based on window size
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

    // Validate mouse pointer position within window bounds
    public Point ValidatePosition(Point position)
    {
        double x = Math.Max(0, Math.Min(position.X, SizeWindow.Width));
        double y = Math.Max(0, Math.Min(position.Y, SizeWindow.Height));

        return new Point(x, y);
    }

    // Mouse pointer event handlers
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
}