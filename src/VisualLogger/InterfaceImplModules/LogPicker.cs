using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules
{
    internal class LogPicker : ILogPicker
    {
        public Task<IEnumerable<string>> PickFromFile()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> PickFromFolder()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> PickFromWebsite()
        {
            throw new NotImplementedException();
        }
    }
}
