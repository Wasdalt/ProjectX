using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models
{
    public class RectangleModel : ViewModelBase
    {
        private double _left;
        private double _top;
        private double _width;
        private double _height;
        private readonly ResultsModel _resultsModel;

        public RectangleModel(ResultsModel resultsModel)
        {
            _resultsModel = resultsModel;
        }

        public double Left
        {
            get => _left;
            set
            {
                this.RaiseAndSetIfChanged(ref _left, value);
                UpdateResults();
            }
        }

        public double Top
        {
            get => _top;
            set
            {
                this.RaiseAndSetIfChanged(ref _top, value);
                UpdateResults();
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                this.RaiseAndSetIfChanged(ref _width, value);
                UpdateResults();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                this.RaiseAndSetIfChanged(ref _height, value);
                UpdateResults();
            }
        }

        public bool HasNonZeroDimensions => Width > 0 && Height > 0;

        private void UpdateResults()
        {
            _resultsModel.Results = $"({Left}, {Top}), {Width}x{Height}";
            this.RaisePropertyChanged(nameof(HasNonZeroDimensions));
        }

        public void Reset()
        {
            Left = Top = Width = Height = 0;
        }

        public void UpdateSizeAndPosition(double previousWidth, double previousHeight, double newWidth, double newHeight)
        {
            Left = Left / previousWidth * newWidth;
            Top = Top / previousHeight * newHeight;
            Width = Width / previousWidth * newWidth;
            Height = Height / previousHeight * newHeight;
        }
    }
}