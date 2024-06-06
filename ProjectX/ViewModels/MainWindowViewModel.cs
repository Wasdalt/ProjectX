using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using ProjectX.Models;
using ProjectX.Models.ExitProgram;
using ProjectX.Models.SelectionToolCropping;
using ProjectX.ViewModels.Page;
using ProjectX.ViewModels.Page.RealizationOCR;
using ProjectX.ViewModels.Page.RealizationSaveImage;
using ProjectX.Views;
using ReactiveUI;
using Point = Avalonia.Point;

namespace ProjectX.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RectangleModel _rectangleModel;
    public PointerModel PointerModel { get; } = new();
    public ResultsModel ResultsModel { get; }
    public PathModel PathModel { get; } = new();
    public SizeWindow SizeWindow { get; } = new();
    public ImageWindow ImageWindow { get; } = new();
    public ImageCropper ImageCropper { get; } = new();

    public RectangleModel RectangleModel => _rectangleModel;
    public IPointerEventHandler PointerEventHandler { get; set; }
    private readonly IShutdownService _shutdownService;
    public Interaction<SecondWindowViewModel, bool> ShowDialogForSecondWindow { get; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowImageCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ShowOCRCommand { get; }

    public MainWindowViewModel()
    {
        ResultsModel = new ResultsModel();
        ShowDialogForSecondWindow = new Interaction<SecondWindowViewModel, bool>();
        _rectangleModel = new RectangleModel(ResultsModel);
        PointerEventHandler = new PointerEventHandler<MainWindowViewModel>(this);
        _shutdownService = ServiceLocator.ShutdownService;

        ShowImageCommand = ReactiveCommand.CreateFromTask(ShowImage);
        CloseCommand = ReactiveCommand.Create(OnCloseCommandExecuted);
        ShowOCRCommand = ReactiveCommand.CreateFromTask(ShowOCR);
    }

    private async Task ShowImage()
    {
        await ShowWindow<SecondWindowViewModel, SaveImagePage>(() => ImageCropper);
    }

    private async Task ShowOCR()
    {
        await ShowWindow<SecondWindowViewModel, OCRPage>(() => ImageCropper);
    }

    private async Task ShowWindow<TViewModel, TPage>(params Func<ViewModelBase>[] createViewModels)
        where TViewModel : SecondWindowViewModel, new()
        where TPage : IPage, new()
    {
        var viewModel = new TViewModel();

        var viewModelProperties = typeof(TViewModel).GetProperties()
            .Where(p => p.PropertyType.IsSubclassOf(typeof(ViewModelBase)))
            .ToDictionary(p => p.PropertyType, p => p);

        foreach (var createViewModel in createViewModels)
        {
            var vm = createViewModel();
            if (viewModelProperties.TryGetValue(vm.GetType(), out var property))
            {
                property.SetValue(viewModel, vm);
            }
        }

        viewModel.NavigateToPage<TPage>();
        await ShowDialogForSecondWindow.Handle(viewModel);
    }

    private void OnCloseCommandExecuted()
    {
        _shutdownService.Shutdown();
    }

    public void ResetToDefault()
    {
        _rectangleModel.Reset();
        PathModel.Reset(SizeWindow.Width, SizeWindow.Height);
        PointerModel.Reset();
    }

    public void WindowResized(double width, double height)
    {
        SizeWindow.UpdateSize(width, height);
        PathModel.UpdateGeometry(SizeWindow.Width, SizeWindow.Height);

        UpdateRectangleSizeAndPosition();
        CutOutRectangleFromPath(_rectangleModel);
    }

    public void WindowImageCorrect()
    {
        // Implement your logic here if needed
    }

    public void CutOutRectangleFromPath(RectangleModel rectangleModel)
    {
        var rectangleGeometry = new RectangleGeometry(new Rect(rectangleModel.Left, rectangleModel.Top,
            rectangleModel.Width, rectangleModel.Height));
        var combinedGeometry =
            new CombinedGeometry(GeometryCombineMode.Exclude, PathModel.OriginalGeometry, rectangleGeometry);

        if (rectangleModel.HasNonZeroDimensions)
        {
            PathModel.Data = combinedGeometry;
        }
    }

    private void UpdateRectangleSizeAndPosition()
    {
        _rectangleModel.UpdateSizeAndPosition(SizeWindow.PreviousWidth, SizeWindow.PreviousHeight, SizeWindow.Width,
            SizeWindow.Height);
    }

    public Point ValidatePosition(Point position)
    {
        return PointerModel.ValidatePosition(position, SizeWindow.Width, SizeWindow.Height);
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
        window.WhenAnyValue(w => w.Bounds).Subscribe(bounds => WindowResized(bounds.Width, bounds.Height));
        window.PositionChanged += (s, e) =>
            SizeWindow.WindowPosition = new PixelPoint(window.Position.X, window.Position.Y);
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        var window = (MainWindow)sender!;
        SizeWindow.Update(window.Width, window.Height, window.Position.X, window.Position.Y);
        WindowResized(SizeWindow.Width, SizeWindow.Height);

        Console.WriteLine($"Initial Size: Width={SizeWindow.Width}, Height={SizeWindow.Height}");
        Console.WriteLine($"Initial Position: X={SizeWindow.WindowPosition.X}, Y={SizeWindow.WindowPosition.Y}");

        WindowImageCorrect();
        window.LayoutUpdated -= OnLayoutUpdated;
    }
}
