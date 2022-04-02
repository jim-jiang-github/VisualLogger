using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceModules
{
    public interface ILogPicker
    {
        string[] FileTypes { get; }

        Task<string[]> PickerLogs();
    }
}
