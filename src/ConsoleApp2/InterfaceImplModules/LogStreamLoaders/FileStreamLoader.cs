using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;

namespace VisualLogger.InterfaceImplModules.LogStreamLoaders
{
    public class FileStreamLoader : LifeCycleTracker<FileStreamLoader>, ILogStreamLoader
    {
        public Stream LoadLogStream(string logPath)
        {
            var stream = File.OpenRead(logPath);
            return stream;
        }
    }
}
