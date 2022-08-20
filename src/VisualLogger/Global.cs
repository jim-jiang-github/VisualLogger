using Microsoft.Extensions.DependencyInjection;

namespace VisualLogger
{
    public class Global
    {
        public static event EventHandler? UIChanged;

        public static IServiceProvider? ServiceProvider { get; set; }

        public static void RefreshUI()
        {
            UIChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void Init()
        {
            ModelDialog.Dialoger = ServiceProvider?.GetService<IModelDialog>();
            Notification.Notifier = ServiceProvider?.GetService<INotification>();
        }
    }
}
