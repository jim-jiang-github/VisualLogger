using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Messages
{
    public interface INotification
    {
        private class NotificationNone : INotification
        {
            public void Show(string msg)
            {

            }
        }

        public static INotification Default = new NotificationNone();
        void Show(string msg);
    }
}
