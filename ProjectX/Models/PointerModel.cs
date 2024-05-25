using Avalonia;
using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models
{
    public class PointerModel : ViewModelBase
    {
        private Point _currentPosition;
        private Point _startPosition;

        public Point CurrentPosition
        {
            get => _currentPosition;
            set => this.RaiseAndSetIfChanged(ref _currentPosition, value);
        }

        public Point StartPosition
        {
            get => _startPosition;
            set => this.RaiseAndSetIfChanged(ref _startPosition, value);
        }
    }
}