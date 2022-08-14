using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Messages
{
    public class ModelDialog
    {
        private static IModelDialog? _dialoger;

        private static IModelDialog DialogerInternal
        {
            get => _dialoger ?? IModelDialog.Default;
        }
        internal static IModelDialog? Dialoger
        {
            set => _dialoger = value;
        }

        public static void Show(string title, string message)
        {
            DialogerInternal.Show(title, message);
        }
    }
}
