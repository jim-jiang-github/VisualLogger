using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.InterfaceModules
{
    public interface ILogPicker
    {
        Task<IEnumerable<string>> PickFromFolder();
        Task<IEnumerable<string>> PickFromWebsite();
        Task<IEnumerable<string>> PickFromFile();
    }
}
