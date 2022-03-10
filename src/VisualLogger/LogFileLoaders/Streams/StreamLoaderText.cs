using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogFileLoaders.Streams
{
    internal class StreamLoaderText : StreamLoader
    {
        public override Stream LoadLogStreamFromPath(string filePath)
        {
            var stream = File.OpenRead(filePath);
            return stream;
        }
    }
}
