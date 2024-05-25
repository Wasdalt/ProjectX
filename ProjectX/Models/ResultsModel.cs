using ProjectX.ViewModels;
using ReactiveUI;

namespace ProjectX.Models
{
    public class ResultsModel : ViewModelBase
    {
        private string _results = "Results...";

        public string Results
        {
            get => _results;
            set => this.RaiseAndSetIfChanged(ref _results, value);
        }
    }
}