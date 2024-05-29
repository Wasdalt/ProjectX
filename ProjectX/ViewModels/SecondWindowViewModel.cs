using System.Drawing;
using ProjectX.Models;
using ReactiveUI;

namespace ProjectX.ViewModels;

public class SecondWindowViewModel : ViewModelBase
{
    private object _currentPage;

    public object CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
    private string _textToDisplay;

    public string TextToDisplay
    {
        get => _textToDisplay;
        set => this.RaiseAndSetIfChanged(ref _textToDisplay, value);
    }

}