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

        public void Error(string error)
        {
            if (_toast == null)
            {
                return;
            }
            var config = new ToastConfig()
            {
                Title = $"An error occurred.",
                Content = error,
                Dark = true,
                Duration = 0,
                Type = AlertTypes.Error
            };
            _toast.AddToast(config);
        }
        public void Warning(string warning)
        {
            if (_toast == null)
            {
                return;
            }
            var config = new ToastConfig()
            {
                Title = $"A warning was found.",
                Content = warning,
                Dark = true,
                Type = AlertTypes.Warning
            };
            _toast.AddToast(config);
        }
        public void Info(string info)
        {
            if (_toast == null)
            {
                return;
            }
            var config = new ToastConfig()
            {
                Title = $"Attention!",
                Content = info,
                Dark = true,
                Type = AlertTypes.Info
            };
            _toast.AddToast(config);
        }
    }
}
