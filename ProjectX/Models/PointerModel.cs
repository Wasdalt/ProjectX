using System;
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
        
        public void Reset()
        {
            StartPosition = default;
            CurrentPosition = default;
        }

        public Point ValidatePosition(Point position, double width, double height)
        {
            return new Point(
                Math.Clamp(position.X, 0, width),
                Math.Clamp(position.Y, 0, height)
            );
        }
    }
}