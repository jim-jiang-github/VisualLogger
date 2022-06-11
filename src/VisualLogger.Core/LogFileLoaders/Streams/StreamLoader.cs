using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Core.LogFileLoaders.Streams
{
    internal abstract class StreamLoader
    {
        public abstract Stream LoadLogStream(string filePath);
    }
}
