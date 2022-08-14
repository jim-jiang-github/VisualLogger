using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Messages
{
    public interface IModelDialog
    {
        private class ModelDialogNone : IModelDialog
        {
            public void Show(string title, string message)
            {

            }
        }
        public static IModelDialog Default = new ModelDialogNone();
        void Show(string title, string message);
    }
}
