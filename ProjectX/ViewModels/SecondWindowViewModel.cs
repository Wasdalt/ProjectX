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

    // public void NavigateToPage(string pageName)
    // {
    //     switch (pageName)
    //     {
    //         case "TextPage":
    //             CurrentPage = new TextPage();
    //             break;
    //         case "ImagePage":
    //             CurrentPage = new ImagePage();
    //             break;
    //     }
    // }
}