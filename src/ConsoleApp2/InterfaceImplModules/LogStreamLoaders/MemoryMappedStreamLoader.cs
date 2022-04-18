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
    public class MemoryMappedStreamLoader : LifeCycleable<MemoryMappedStreamLoader>, ILogStreamLoader
    {
        public Stream LoadLogStream(string logPath)
        {
            using MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(logPath);
            var stream = memoryMappedFile.CreateViewStream();
            return stream;
        }
    }
}
