using System;
using System.Collections.Generic;
using Avalonia;
using ProjectX.Views;

namespace ProjectX.Models;

public static class ScreenHelper
{

    public static int GetScreenCount()
    {
        var window = new MainWindow();
        return window.Screens.All.Count;
    }
    
    [Obsolete("Obsolete")]
    public static IEnumerable<ScreenInfo> GetScreensInfo()
    {
        var window = new MainWindow();
        var screensInfo = new List<ScreenInfo>();

        foreach (var screen in window.Screens.All)
        {
            screensInfo.Add(new ScreenInfo
            {
                Bounds = screen.Bounds,
                WorkingArea = screen.WorkingArea,
                PixelDensity = screen.PixelDensity
            });
        }

        return screensInfo;
    }
}

public class ScreenInfo
{
    public PixelRect Bounds { get; set; }
    public PixelRect WorkingArea { get; set; }
    public double PixelDensity { get; set; }
}