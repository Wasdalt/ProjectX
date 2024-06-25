namespace ProjectX.ViewModels.Page;

public interface IPageFactory
{
    IPage CreatePage<T>() where T : IPage, new();
}