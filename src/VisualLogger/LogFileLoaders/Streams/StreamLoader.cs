using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogFileLoaders.Streams
{
    internal abstract class StreamLoader
    {
        public abstract Stream LoadLogStreamFromPath(string filePath);
    }
}
