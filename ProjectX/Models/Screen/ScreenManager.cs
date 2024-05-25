using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using ProjectX.Views;

namespace ProjectX.Models.Screen
{
    public class ScreenManager
    {
        private static readonly Lazy<ScreenManager> _instance = new(() => new ScreenManager());
        public static ScreenManager Instance => _instance.Value;

        private readonly MainWindow _mainWindow;

        private ScreenManager()
        {
            // Создаем экземпляр MainWindow при создании ScreenManager
            _mainWindow = new MainWindow();
        }

        public IReadOnlyList<Avalonia.Platform.Screen> GetAllScreens()
        {
            if (_mainWindow.Screens.All.Any())
            {
                return new List<Avalonia.Platform.Screen>(_mainWindow.Screens.All);
            }

            return new List<Avalonia.Platform.Screen>();
        }

        public Avalonia.Platform.Screen? GetPrimaryScreen()
        {
            return GetAllScreens().FirstOrDefault(s => s.IsPrimary);
        }
    }
}
