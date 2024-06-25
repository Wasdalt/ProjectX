using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models
{
    public class ResultsModel : ViewModelBase
    {
        private string _results = "Результат...";

        public string Results
        {
            get => _results;
            set => this.RaiseAndSetIfChanged(ref _results, value);
        }
    }
}