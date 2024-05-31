using Avalonia;
using Avalonia.Media;
using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models;

public class PathModel : ViewModelBase
{
    private Geometry _originalGeometry = null!;
    private Geometry _data = null!;

    public Geometry OriginalGeometry
    {
        get => _originalGeometry;
        set => this.RaiseAndSetIfChanged(ref _originalGeometry, value);
    }

    public Geometry Data
    {
        get => _data;
        set => this.RaiseAndSetIfChanged(ref _data, value);
    }

    public void Reset(double width, double height)
    {
        OriginalGeometry = new RectangleGeometry(new Rect(0, 0, width, height));
        Data = OriginalGeometry;
    }

    public void UpdateGeometry(double width, double height)
    {
        OriginalGeometry = new RectangleGeometry(new Rect(0, 0, width, height));
        Data = OriginalGeometry;
    }
}
