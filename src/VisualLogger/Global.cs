using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VisualLogger.Localization;
using VisualLogger.Storage;

namespace VisualLogger
{
    public class Global
    {
        public static event EventHandler? UIChanged;

        private static IFileStorage? _fileStorage;

        public static IServiceProvider? ServiceProvider { get; set; }
        public static IFileStorage FileStorage => _fileStorage ??= (ServiceProvider?.GetService<IFileStorage>() ?? IFileStorage.Default);

        public static string CurrentAppDataDirectory => Path.Combine(FileStorage.AppDataDirectory, nameof(VisualLogger));

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
