using Avalonia.Input;

namespace ProjectX.ViewModels;

public interface IPointerEventHandler
{
    void HandlePointerPressed(PointerPressedEventArgs args);
    void HandlePointerMoved(PointerEventArgs args);
    void HandlePointerReleased(PointerReleasedEventArgs args);
}