using MvvmBlazor;
using MvvmBlazor.ViewModel;

namespace VisualLogger.Viewer.Web.ViewModels
{
    public partial class DialogViewModel : ViewModelBase
    {
        [Notify]
        private bool _isOpen = false;

        public void IsOpenChanged(bool isOpen)
        {
            _isOpen = isOpen;
        }
    }
}
