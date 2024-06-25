using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace ProjectX.ViewModels.Page.RealizationSaveImage;

public partial class SaveImagePage : UserControl, IPage
{
    public SaveImagePage()
    {
        InitializeComponent();
    }
    private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            var image = this.FindControl<Image>("ImageControl");
            if (image.RenderTransform is ScaleTransform scaleTransform)
            {
                var cursorPosition = e.GetPosition(image);
                var relativeX = cursorPosition.X / image.Bounds.Width;
                var relativeY = cursorPosition.Y / image.Bounds.Height;

                image.RenderTransformOrigin = new RelativePoint(relativeX, relativeY, RelativeUnit.Relative);

                double zoomFactor = 0.1;
                if (e.Delta.Y > 0)
                {
                    scaleTransform.ScaleX += zoomFactor;
                    scaleTransform.ScaleY += zoomFactor;
                }
                else if (e.Delta.Y < 0)
                {
                    scaleTransform.ScaleX -= zoomFactor;
                    scaleTransform.ScaleY -= zoomFactor;
                }

  
                scaleTransform.ScaleX = Math.Max(0.1, scaleTransform.ScaleX);
                scaleTransform.ScaleY = Math.Max(0.1, scaleTransform.ScaleY);

                e.Handled = true; 
            }
        }
    }

    private void OnScrollViewerPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = false; 
        }
        else
        {
            e.Handled = true;
        }
    }
}