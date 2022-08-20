using BlazorComponent;
using Masa.Blazor.Presets;
using MvvmBlazor;
using MvvmBlazor.ViewModel;
using VisualLogger.Messages;

namespace VisualLogger.Viewer.ViewModels
{
    public partial class ModelDialogContainerViewModel : DialogViewModel, IModelDialog
    {
        [Notify]
        private string? _title;
        [Notify]
        private string? _message;

        public void Show(string title, string message)
        {
            Title = title;
            Message = message;

            IsOpen = true;
        }
    }
}
