using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using ProjectX.ViewModels;
using ProjectX.Views;

namespace ProjectX.Models;

public class PointerEventHandler<T> : IPointerEventHandler where T : MainWindowViewModel
{
    private readonly T _currentViewModel;
    private readonly List<T> _allViewModels;
    // private readonly IScreenshotService<double> _screenshotService;

    public PointerEventHandler(T currentViewModel, List<T>? allViewModels = null)
    {
        _currentViewModel = currentViewModel;
        _allViewModels = allViewModels ?? new List<T>();
        // _screenshotService = ServiceLocator.GetService<IScreenshotService<double>>();
    }

    public void HandlePointerPressed(PointerPressedEventArgs args)
    {
        foreach (var viewModel in _allViewModels)
        {
            if (viewModel != _currentViewModel)
            {
                viewModel.ResetToDefault();
            }
        }
        _currentViewModel.PathModel.Data = _currentViewModel.PathModel.OriginalGeometry;
        _currentViewModel.RectangleModel.Left = _currentViewModel.RectangleModel.Top = _currentViewModel.RectangleModel.Width = _currentViewModel.RectangleModel.Height = 0;
        _currentViewModel.PointerModel.StartPosition = _currentViewModel.PointerModel.CurrentPosition = _currentViewModel.ValidatePosition(args.GetCurrentPoint(null).Position);
    }

    public void HandlePointerMoved(PointerEventArgs args)
    {
        _currentViewModel.PointerModel.CurrentPosition = _currentViewModel.ValidatePosition(args.GetCurrentPoint(null).Position);

        if (args.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        {
            double width = Math.Abs(_currentViewModel.PointerModel.CurrentPosition.X - _currentViewModel.PointerModel.StartPosition.X);
            double height = Math.Abs(_currentViewModel.PointerModel.CurrentPosition.Y - _currentViewModel.PointerModel.StartPosition.Y);

            _currentViewModel.ResultsModel.Results =
                $"Start: ({_currentViewModel.PointerModel.StartPosition.X}, {_currentViewModel.PointerModel.StartPosition.Y}), Width: {width}, Height: {height}";
            _currentViewModel.RectangleModel.Width = width;
            _currentViewModel.RectangleModel.Height = height;
            _currentViewModel.RectangleModel.Left = Math.Min(_currentViewModel.PointerModel.StartPosition.X, _currentViewModel.PointerModel.CurrentPosition.X);
            _currentViewModel.RectangleModel.Top = Math.Min(_currentViewModel.PointerModel.StartPosition.Y, _currentViewModel.PointerModel.CurrentPosition.Y);

            _currentViewModel.CutOutRectangleFromPath(_currentViewModel.RectangleModel);
        }
    }

    public void HandlePointerReleased(PointerReleasedEventArgs args)
    {
        _currentViewModel.ResultsModel.Results =
            $"Start: ({_currentViewModel.PointerModel.StartPosition.X}, {_currentViewModel.PointerModel.StartPosition.Y}), Width: {_currentViewModel.RectangleModel.Width}, Height: {_currentViewModel.RectangleModel.Height}";
        
        int startX = (int)_currentViewModel.RectangleModel.Left;
        int startY = (int)_currentViewModel.RectangleModel.Top;
        int width = (int)_currentViewModel.RectangleModel.Width;
        int height = (int)_currentViewModel.RectangleModel.Height;
        // _currentViewModel.ImageWindow.ImageFromBindingPath
        // Crop the screenshot using the screenshot service
        string croppedImagePath = ScreenshotCropperWindow.CropScreenshotWindow(_currentViewModel.ImageWindow.ImageFromBindingPath, startX, startY, width, height);
        
        string croppedImageName = Path.GetFileName(croppedImagePath);
        
        // Optionally, update the view model with the new cropped image name
        if (!string.IsNullOrEmpty(croppedImageName))
        {
            // _currentViewModel.CroppedImageName = croppedImageName;
        }
    }
}
