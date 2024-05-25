using Avalonia.Media;
using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models;

public class PathModel : ViewModelBase
{
    private Geometry _originalGeometry = null!;

    public Geometry OriginalGeometry
    {
        get => _originalGeometry;
        set => this.RaiseAndSetIfChanged(ref _originalGeometry, value);
    }

    private Geometry _data = null!;

    public Geometry Data
    {
        get => _data;
        set => this.RaiseAndSetIfChanged(ref _data, value);
    }
}