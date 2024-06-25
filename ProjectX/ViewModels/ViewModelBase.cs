using System;
using ProjectX.ViewModels.Page;
using ProjectX.Views;
using ReactiveUI;

namespace ProjectX.ViewModels;

public class ViewModelBase : ReactiveObject
{
    protected INavigationService NavigationService => ServiceLocator.NavigationService;

    public virtual void NavigateToPage<TPage>() where TPage : IPage, new()
    {
        NavigationService.NavigateTo<TPage>();
    }
}
public interface INavigationService
{
    void NavigateTo<TPage>() where TPage : IPage, new();
}

public class NavigationService : INavigationService
{
    public void NavigateTo<TPage>() where TPage : IPage, new()
    {
        // Implement navigation logic here
        // For example, set the current page in a frame or a content control
        Console.WriteLine($"Navigating to {typeof(TPage).Name}");
    }
}

