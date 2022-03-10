using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Shared.InterfaceModules
{
    public interface IFilesPicker
    {
        Task<IEnumerable<string>> PickFiles();
    }
}
