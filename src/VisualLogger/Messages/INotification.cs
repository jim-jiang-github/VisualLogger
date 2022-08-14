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
            public void Error(string error)
            {

            }
            public void Warning(string warning)
            {

            }
            public void Info(string info)
            {

            }
        }

        public static INotification Default = new NotificationNone();

        void Error(string error);
        void Warning(string warning);
        void Info(string info);
    }
}
