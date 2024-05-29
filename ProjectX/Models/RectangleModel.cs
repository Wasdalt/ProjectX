using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models
{
    public class RectangleModel(ResultsModel resultsModel) : ViewModelBase
    {
        private double _left;
        private double _top;
        private double _width;
        private double _height;

        public double Left
        {
            get => _left;
            set
            {
                this.RaiseAndSetIfChanged(ref _left, value);
                this.RaisePropertyChanged(nameof(HasNonZeroDimensions));
                UpdateResults();
            }
        }

        public double Top
        {
            get => _top;
            set
            {
                this.RaiseAndSetIfChanged(ref _top, value);
                this.RaisePropertyChanged(nameof(HasNonZeroDimensions));
                UpdateResults();
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                this.RaiseAndSetIfChanged(ref _width, value);
                this.RaisePropertyChanged(nameof(HasNonZeroDimensions));
                UpdateResults();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                this.RaiseAndSetIfChanged(ref _height, value);
                this.RaisePropertyChanged(nameof(HasNonZeroDimensions));
                UpdateResults();
            }
        }

        public bool HasNonZeroDimensions => _width > 0 && _height > 0;

        private void UpdateResults()
        {
            resultsModel.Results = $"({Left}, {Top}), {Width}x{Height}";
        }
    }
}