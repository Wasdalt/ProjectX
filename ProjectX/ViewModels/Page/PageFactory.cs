namespace ProjectX.ViewModels.Page;

public class PageFactory : IPageFactory
{
    public IPage CreatePage<T>() where T : IPage, new()
    {
        return new T();
    }
}