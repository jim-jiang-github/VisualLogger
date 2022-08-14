using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Messages
{
    public class Notification
    {
        private static INotification? _notifier;

        private static INotification NotifierInternal
        {
            get => _notifier ?? INotification.Default;
        }
        internal static INotification? Notifier
        {
            set => _notifier = value;
        }

        public static void Error(string error)
        {
            NotifierInternal.Error(error);
        }
        public static void Warning(string warning)
        {
            NotifierInternal.Warning(warning);
        }
        public static void Info(string info)
        {
            NotifierInternal.Info(info);
        }
    }
}
