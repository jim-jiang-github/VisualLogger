using BlazorComponent;
using Masa.Blazor.Presets;
using VisualLogger.Messages;
using VisualLogger.Viewer.Web.Localization;

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
                Title = I18nKeys.Notification.ErrorTitle,
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
                Title = I18nKeys.Notification.WarningTitle,
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
                Title = I18nKeys.Notification.InfoTitle,
                Content = info,
                Dark = true,
                Type = AlertTypes.Info
            };
            _toast.AddToast(config);
        }
    }
}
