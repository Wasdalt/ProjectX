using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProjectX.ViewModels.Page.Settings;

public partial class SettingsPage : UserControl, IPage
{

    public SettingsPage()
    {
        InitializeComponent();
    }
    
    
    private void OnTextBoxGotFocus(object sender, GotFocusEventArgs e)
    {
        if (DataContext is SecondWindowViewModel viewModel)
        {
            viewModel.KeySettings.OnTextBoxGotFocus();
        }
    }

    private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SecondWindowViewModel viewModel)
        {
            viewModel.KeySettings.OnTextBoxLostFocus();
        }
    }

    private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is SecondWindowViewModel viewModel)
        {
            viewModel.KeySettings.OnTextBoxKeyDown(e);
        }
    }
}