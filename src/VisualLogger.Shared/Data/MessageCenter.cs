using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Shared.Data
{
    public delegate void NotificationMessageHandler(string title, string message);
    public static class MessageCenter
    {
        public static event NotificationMessageHandler? NotificationSuccess;

        public static void NotifySuccess(string title, string message)
        {
            NotificationSuccess?.Invoke(title, message);
        }
    }
}
