using Avalonia.Media.Imaging;
using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models;

public class ImageWindow : ViewModelBase
{
    private Bitmap _image = null!;
    
    public Bitmap ImageFromBinding 
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }
    
    private string _imagePath = null!;
    
    public string ImageFromBindingPath
    {
        get =>  _imagePath;
        set => this.RaiseAndSetIfChanged(ref  _imagePath, value);
    }
}