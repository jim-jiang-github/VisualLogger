using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules
{
    public class LogContentLoader : ILogContentLoader
    {
        public ILogContent LoadLogContent(Stream stream, ILogSchemaLoader logSchemaLoader)
        {
            throw new NotImplementedException();
        }
    }
}
