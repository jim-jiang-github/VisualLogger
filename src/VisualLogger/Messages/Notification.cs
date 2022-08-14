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

        public static void Show(string msg) 
        {
            NotifierInternal.Show(msg);
        }
    }
}
