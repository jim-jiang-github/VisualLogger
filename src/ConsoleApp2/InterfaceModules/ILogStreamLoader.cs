using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.InterfaceModules
{
    public interface ILogStreamLoader
    {
        Stream LoadLogStream(string logPath);
    }
}
