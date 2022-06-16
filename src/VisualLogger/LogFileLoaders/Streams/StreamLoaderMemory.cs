using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogFileLoaders.Streams
{
    internal class StreamLoaderMemory : StreamLoader
    {
        public override Stream LoadLogStreamFromPath(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
