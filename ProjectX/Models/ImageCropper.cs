using System;
using Avalonia.Media.Imaging;
using ProjectX.ViewModels;
namespace ProjectX.Models;
using ReactiveUI;

public class ImageCropper : ViewModelBase
{
    private Bitmap? _image;

    public Bitmap? ImageCropperFromBinding
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    private string? _imagePath;

    public string? ImageCropperFromBindingPath
    {
        get => _imagePath;
        set
        {
            this.RaiseAndSetIfChanged(ref _imagePath, value);
            UpdateImage();
        }
    }

    private void UpdateImage()
    {
        if (string.IsNullOrEmpty(_imagePath))
        {
            ImageCropperFromBinding = null;
        }
        else
        {
            try
            {
                ImageCropperFromBinding = new Bitmap(_imagePath);
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                ImageCropperFromBinding = null;
            }
        }
    }
}
