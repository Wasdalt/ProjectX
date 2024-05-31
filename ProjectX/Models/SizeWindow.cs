using Avalonia;
using Avalonia.Media.Imaging;
using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models;

public class SizeWindow : ViewModelBase
{
    private double _width;
    private double _height;
    private PixelPoint _windowPosition;

    public double PreviousWidth { get; set; }
    public double PreviousHeight { get; set; }

    public double Width
    {
        get => _width;
        set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    public double Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    public PixelPoint WindowPosition
    {
        get => _windowPosition;
        set => this.RaiseAndSetIfChanged(ref _windowPosition, value);
    }

    public void UpdateSize(double width, double height)
    {
        PreviousWidth = Width;
        PreviousHeight = Height;
        Width = width;
        Height = height;
    }

    public void Update(double width, double height, int x, int y)
    {
        UpdateSize(width, height);
        WindowPosition = new PixelPoint(x, y);
    }
}