using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogFileLoaders.Streams
{
    internal class StreamLoaderMemoryMapped : StreamLoader
    {
        public override Stream LoadLogStreamFromPath(string filePath)
        {
            using MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(filePath);
            var stream = memoryMappedFile.CreateViewStream();
            return stream;
        }
    }
}
