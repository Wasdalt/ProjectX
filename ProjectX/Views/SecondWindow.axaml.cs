using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectX.ViewModels;
using ProjectX.ViewModels.Page;
using ReactiveUI;

namespace ProjectX.Views;

public partial class SecondWindow : ReactiveWindow<SecondWindowViewModel>
{
    // SecondWindowViewModel.SecondWindowInstance = this;
    // public static SecondWindow Instance { get; private set; }
    public SecondWindow()
    {
        InitializeComponent();
        DataContextChanged += (s, e) =>
        {
            if (DataContext is SecondWindowViewModel viewModel)
            {
                viewModel.SubscribeToEvents(this);
            }
        };
        // this.WhenActivated(d => { d(ViewModel!.ShowSaveFileDialog.RegisterHandler(DoSaveFileDialogAsync)); });
    }


    // private async Task DoSaveFileDialogAsync(InteractionContext<Unit, string?> interaction)
    // {
    //     var saveFileDialog = new SaveFileDialog
    //     {
    //         DefaultExtension = "txt",
    //         Filters = new List<FileDialogFilter>
    //         {
    //             new FileDialogFilter { Name = "Text Files", Extensions = new List<string> { "txt" } },
    //             new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
    //         }
    //     };
    //
    //     var result = await saveFileDialog.ShowAsync(this);
    //
    //     interaction.SetOutput(result);
    // }
}