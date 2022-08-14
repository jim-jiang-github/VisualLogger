using BlazorComponent;
using Masa.Blazor.Presets;
using VisualLogger.Messages;

namespace VisualLogger.Viewer.Web.ViewModels
{
    public class NotificationContainerViewModel : INotification
    {
        private PToast? _toast;

        public void SetCurrentToast(PToast? toast)
        {
            _toast = toast;
        }

        public void Show(string msg)
        {
            if (_toast == null)
            {
                return;
            }
            var now = DateTime.Now.ToString("HH:mm:ss.fff");
            var config = new ToastConfig()
            {
                Title = $"Add Toast by Click",
                Content = $"create time: {now}",
                Dark = true,
                Type = AlertTypes.Error
            };

            _toast.AddToast(config);
        }
    }
}
