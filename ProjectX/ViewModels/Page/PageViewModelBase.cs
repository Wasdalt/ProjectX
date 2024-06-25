using ReactiveUI;

namespace ProjectX.ViewModels.Page;

public abstract class PageViewModelBase : ViewModelBase
{
    private IPage _currentPage = null!;

    public IPage CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
}